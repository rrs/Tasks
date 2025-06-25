using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rrs.Tasks;

/// <summary>
/// Ensures only one task is running for a given key. All callers during execution will receive the same result. On completion the task is removed.
/// </summary>
public class TaskCache<TKey>
{
    private readonly ConcurrentDictionary<TKey, ICachedTask> _taskCache = new();

    public async Task<T> GetOrStart<T>(TKey key, Func<Task<T>> taskFactory)
    {
        var cachedTask = new CachedTask<T>(taskFactory);
        cachedTask = (CachedTask<T>)await GetOrStart(key, cachedTask);
        return await cachedTask.Task.Value;
    }

    public async Task RunOrStart(TKey key, Func<Task> taskFactory)
    {
        var cachedTask = new CachedTask(taskFactory);
        await GetOrStart(key, cachedTask);
    }

    private async Task<ICachedTask> GetOrStart(TKey key, ICachedTask cachedTask)
    {
        object executionId = cachedTask.Id; // take a ref to this CachedTask Id
        try
        {
            // multiple threads create instances of CachedTask which is cheap. Only the one added to the dictionary will be used and returned
            cachedTask = _taskCache.GetOrAdd(key, cachedTask);
            // invoking the cachedtask's value starts the task, and only once
            await cachedTask.Task;
            return cachedTask;
        }
        finally
        {
            // if this was the context which added it then remove it.
            if (cachedTask?.Id == executionId)
            {
                _taskCache.TryRemove(key, out var _);
            }
        }
    }

    private interface ICachedTask
    {
        object Id { get; }
        Task Task { get; }
    }

    // keeps track of a task and the context it was started in
    private class CachedTask : ICachedTask
    {
        public object Id { get; } = new();
        public Lazy<Task> Task { get; } // lazy so the task is only run once

        Task ICachedTask.Task => Task.Value; 

        public CachedTask(Func<Task> task)
        {
            Task = new Lazy<Task>(task);
        }
    }

    private class CachedTask<T> : ICachedTask
    {
        public object Id { get; } = new();
        public Lazy<Task<T>> Task { get; }
        
        Task ICachedTask.Task => Task.Value;

        public CachedTask(Func<Task<T>> task)
        {
            Task = new Lazy<Task<T>>(task);
        }
    }
}

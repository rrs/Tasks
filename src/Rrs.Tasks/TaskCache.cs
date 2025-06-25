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
        var executionId = new object(); // take a unique reference to this scope
        var cachedTask = new CachedTask<T>(executionId, taskFactory);
        cachedTask = (CachedTask<T>)await GetOrStart(key, executionId, cachedTask);
        return await cachedTask.Task.Value;
    }

    public async Task RunOrStart(TKey key, Func<Task> taskFactory)
    {
        var executionId = new object(); // take a unique reference to this scope
        var cachedTask = new CachedTask(executionId, taskFactory);
        await GetOrStart(key, executionId, cachedTask);
    }

    private async Task<ICachedTask> GetOrStart(TKey key, object executionId, ICachedTask cachedTask)
    {
        try
        {
            // multiple threads could create instances of cachedtask which is cheap. Only the one added to the dictionary will be used
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
        public object Id { get; }
        public Lazy<Task> Task { get; } // lazy so the task is only run once
        Task ICachedTask.Task => Task.Value; 

        public CachedTask(object id, Func<Task> task)
        {
            Id = id;
            Task = new Lazy<Task>(task);
        }
    }

    // keeps track of a task and the context it was started in
    private class CachedTask<T> : ICachedTask
    {
        public object Id { get; }
        public Lazy<Task<T>> Task { get; } // lazy so the task is only run once
        
        Task ICachedTask.Task => Task.Value;

        public CachedTask(object id, Func<Task<T>> task)
        {
            Id = id;
            Task = new Lazy<Task<T>>(task);
        }
    }
}

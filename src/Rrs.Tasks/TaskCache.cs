using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rrs.Tasks;

/// <summary>
/// Ensures only one task is running for a given key. All callers during execution will receive the same result. On completion the task is removed.
/// </summary>
public class TaskCache<TKey>
{
    private readonly ConcurrentDictionary<TKey, CachedTask> _taskCache = new ConcurrentDictionary<TKey, CachedTask>();

    public async Task<T> GetOrStart<T>(TKey key, Func<Task<T>> taskFactory)
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        await GetOrStart(key, async () =>
        {
            try
            {
                var result = await taskFactory();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return await tcs.Task;
    }

    public Task GetOrStart(TKey key, Func<Task> taskFactory)
    {
        var executionId = new object(); // take a unique reference to this scope
        CachedTask cachedTask = null;
        try
        {
            // multiple threads could create instances of cachedtask which is cheap. Only the one added to the dictionary will be used
            cachedTask = _taskCache.GetOrAdd(key, new CachedTask(executionId, taskFactory));
            // invoking the cachedtask's value starts the task, and only once
            return cachedTask.Task.Value;
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

    // keeps track of a task and the context it was started in
    private class CachedTask
    {
        public object Id { get; }
        public Lazy<Task> Task { get; } // lazy so the task is only run once

        public CachedTask(object id, Func<Task> task)
        {
            Id = id;
            Task = new Lazy<Task>(task);
        }
    }
}

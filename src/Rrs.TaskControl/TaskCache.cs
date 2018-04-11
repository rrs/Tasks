using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    /// <summary>
    /// Ensures only one task is running for a given key. All callers during execution will receive the same result. On completion the task is removed.
    /// </summary>
    public class TaskCache<TKey, TTask>
    {
        private readonly ConcurrentDictionary<TKey, CachedTask<TTask>> _taskCache = new ConcurrentDictionary<TKey, CachedTask<TTask>>();

        public Task<TTask> GetOrStart(TKey key, Func<Task<TTask>> taskFactory)
        {
            var executionId = new object(); // take a unique reference to this scope
            CachedTask<TTask> cachedTask = null;
            try
            {
                // multiple threads could create instances of cachedtask which is cheap. Only the one added to the dictionary will be used
                cachedTask = _taskCache.GetOrAdd(key, new CachedTask<TTask>(executionId, taskFactory));
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
        private class CachedTask<T>
        {
            public object Id { get; set; }
            public Lazy<Task<T>> Task { get; set; } // lazy so the task is only run once

            public CachedTask(object id, Func<Task<T>> task)
            {
                Id = id;
                Task = new Lazy<Task<T>>(task);
            }
        }
    }
}

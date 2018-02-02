using Rrs.TaskControl.Pulsable;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    public class TaskQueue : IDisposable
    {
        private readonly ConcurrentQueue<IDoSomeWork> _queue = new ConcurrentQueue<IDoSomeWork>();

        private readonly PulseWorker _pw;

        public TaskQueue(ITaskQueueConsumer taskQueueConsumer = null)
        {
            if (taskQueueConsumer == null) taskQueueConsumer = new TaskQueueConsumer();
            _pw = new PulseWorker(() => taskQueueConsumer.ConsumeQueue(_queue));
        }

        public void Dispose()
        {
            _pw.Dispose();
        }

        public Task Enqueue(Action a)
        {
            var ww = new SyncWorkWrapper(a);
            _queue.Enqueue(ww);
            _pw.Pulse();
            return ww.Task;
        }

        public Task<T> Enqueue<T>(Func<T> f)
        {
            var ww = new SyncWorkWrapper<T>(f);
            _queue.Enqueue(ww);
            _pw.Pulse();
            return ww.Task;
        }

        public Task Enqueue(Func<Task> a)
        {
            var ww = new AsyncWorkWrapper(a);
            _queue.Enqueue(ww);
            _pw.Pulse();
            return ww.Task;
        }

        public Task<T> Enqueue<T>(Func<Task<T>> f)
        {
            var ww = new AsyncWorkWrapper<T>(f);
            _queue.Enqueue(ww);
            _pw.Pulse();
            return ww.Task;
        }
    }
}

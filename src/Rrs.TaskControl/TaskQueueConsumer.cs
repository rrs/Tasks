using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    class TaskQueueConsumer : ITaskQueueConsumer
    {
        public Task ConsumeQueue(ConcurrentQueue<IDoSomeWork> queue)
        {
            var tcs = new TaskCompletionSource<object>();
            ConsumeQueueInternal(queue, tcs);
            return tcs.Task;
        }

        private void ConsumeQueueInternal(ConcurrentQueue<IDoSomeWork> queue, TaskCompletionSource<object> tcs)
        {
            if (queue.TryDequeue(out var workWrapper))
            {
                workWrapper.Execute().ContinueWith(t => ConsumeQueue(queue));
            }
            else
            {
                tcs.SetResult(null);
            }
        }
    }
}

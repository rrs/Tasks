using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    internal class TaskQueueConsumer : ITaskQueueConsumer
    {
        public Task ConsumeQueue(ConcurrentQueue<IDoSomeWork> queue, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>();
            ConsumeQueueInternal(queue, tcs, token);
            return tcs.Task;
        }

        private void ConsumeQueueInternal(ConcurrentQueue<IDoSomeWork> queue, TaskCompletionSource<object> tcs, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                tcs.SetCanceled();
                return;
            };
            if (queue.TryDequeue(out var workWrapper))
            {
                workWrapper.Execute(token).ContinueWith(t => ConsumeQueueInternal(queue, tcs, token));
            }
            else
            {
                tcs.SetResult(null);
            }
        }
    }
}

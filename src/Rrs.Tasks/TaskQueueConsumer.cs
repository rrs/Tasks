using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

internal class TaskQueueConsumer : ITaskQueueConsumer
{
    public async Task ConsumeQueue(ConcurrentQueue<IDoSomeWork> queue, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        };
        while(queue.TryDequeue(out var workWrapper))
        {
            await workWrapper.Execute(token);
        }
    }
}

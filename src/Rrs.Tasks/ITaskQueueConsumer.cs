using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks
{
    public interface ITaskQueueConsumer
    {
        Task ConsumeQueue(ConcurrentQueue<IDoSomeWork> queue, CancellationToken token);
    }
}

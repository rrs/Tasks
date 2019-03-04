using System.Collections.Concurrent;
using System.Threading;

namespace Rrs.Tasks
{
    public interface ITaskQueuePulsable
    {
        void OnPulse(ITaskQueueConsumer taskQueueConsumer, ConcurrentQueue<IDoSomeWork> queue, CancellationTokenSource cancellationTokenSource);
    }
}

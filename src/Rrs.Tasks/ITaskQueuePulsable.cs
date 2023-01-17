using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public interface ITaskQueuePulsable
{
    Task OnPulse(ITaskQueueConsumer taskQueueConsumer, ConcurrentQueue<IDoSomeWork> queue, CancellationTokenSource cancellationTokenSource);
}

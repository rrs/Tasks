using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    public interface ITaskQueueConsumer
    {
        Task ConsumeQueue(ConcurrentQueue<IDoSomeWork> queue);
    }
}

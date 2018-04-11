using System.Threading;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    public interface IDoSomeWork
    {
        Task Execute(CancellationToken token);
    }
}

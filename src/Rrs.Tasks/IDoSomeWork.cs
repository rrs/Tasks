using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public interface IDoSomeWork
{
    Task Execute(CancellationToken token);
}

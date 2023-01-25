using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public interface IDoAsync
{
    Task Do(CancellationToken cancellationToken);
}

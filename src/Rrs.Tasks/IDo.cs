using System.Threading;

namespace Rrs.Tasks;

public interface IDo
{
    void Do(CancellationToken cancellationToken);
}

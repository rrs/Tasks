using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

class DoActionWrapper : IDoAsync
{
    private readonly IDo _r;

    public DoActionWrapper(IDo r) => _r = r;

    public Task Do(CancellationToken cancellationToken)
    {
        _r.Do(cancellationToken);
        return Task.CompletedTask;
    }
}

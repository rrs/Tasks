using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

class DoActionAsync : IDoAsync
{
    private readonly Func<CancellationToken, Task> _f;
    public DoActionAsync(Func<CancellationToken, Task> f) => _f = f;
    public Task Do(CancellationToken cancellationToken) => _f(cancellationToken);
}

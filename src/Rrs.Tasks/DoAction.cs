using System;
using System.Threading;

namespace Rrs.Tasks;

class DoAction : IDo
{
    private readonly Action<CancellationToken> _a;
    public DoAction(Action<CancellationToken> a) => _a = a;
    public void Do(CancellationToken cancellationToken) => _a(cancellationToken);
}

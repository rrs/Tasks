using System;
using System.Threading;

namespace Rrs.Tasks;

class RepeatedAction : IRepeat
{
    public TimeSpan Rate { get; }
    private readonly Action<CancellationToken> _a;
    public RepeatedAction(TimeSpan rate, Action<CancellationToken> a)
    {
        Rate = rate;
        _a = a;
    }

    public void OnRepeat(CancellationToken cancellationToken) => _a(cancellationToken);
}

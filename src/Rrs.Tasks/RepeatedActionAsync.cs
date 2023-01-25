using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

class RepeatedActionAsync : IRepeatAsync
{
    public TimeSpan Rate { get; }

    private readonly Func<CancellationToken, Task> _f;
    public RepeatedActionAsync(TimeSpan rate, Func<CancellationToken, Task> f)
    {
        Rate = rate;
        _f = f;
    }

    public Task OnRepeat(CancellationToken cancellationToken) =>_f(cancellationToken);
}

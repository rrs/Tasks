using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

class RepeatedActionWrapper : IRepeatAsync
{
    private readonly IRepeat _r;
    public TimeSpan Rate => _r.Rate;

    public RepeatedActionWrapper(IRepeat r)
    {
        _r = r;
    }

    public Task OnRepeat(CancellationToken cancellationToken)
    {
        _r.OnRepeat(cancellationToken);
        return Task.CompletedTask;
    }
}

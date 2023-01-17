using System;
using System.Threading;

namespace Rrs.Tasks;

public class ScheduledTask
{
    private readonly Action _action;
    private CancellationTokenSource _cts;

    public ScheduledTask(Action action)
    {
        _action = action;
    }

    public void ScheduleIn(TimeSpan timeSpan)
    {
        var cts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        cts?.Cancel();
        Schedule.In(_action, timeSpan, _cts.Token);
    }

    public void ScheduleAt(DateTime datetime)
    {
        var timeSpan = datetime - DateTime.Now;
        var cts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        cts?.Cancel();
        Schedule.In(_action, timeSpan, _cts.Token);
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }
}

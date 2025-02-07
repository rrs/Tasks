using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public static class Schedule
{
    public static Task In(Action a, TimeSpan timespan, CancellationToken token = default)
    {
        return Task.Run(async () =>
        {
            await Task.Delay((int)timespan.TotalMilliseconds, token);
            if (token.IsCancellationRequested) return;
            a();
        });
    }

    public static Task In(Func<Task> a, TimeSpan timespan, CancellationToken token = default)
    {
        return Task.Run(async () =>
        {
            await Task.Delay((int)timespan.TotalMilliseconds, token);
            if (token.IsCancellationRequested) return;
            await a();
        });
    }
}

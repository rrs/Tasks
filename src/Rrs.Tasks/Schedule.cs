using System;
using System.Threading;

namespace Rrs.Tasks
{
    public static class Schedule
    {
        public static void In(Action a, TimeSpan timespan, CancellationToken token = default(CancellationToken))
        {
            Timer timer = null;
            timer = new Timer(_ =>
            {
                timer.Dispose();
                if (token.IsCancellationRequested) return;
                a();
            });
            
            timer.Change((int)timespan.TotalMilliseconds, Timeout.Infinite);
        }
    }
}

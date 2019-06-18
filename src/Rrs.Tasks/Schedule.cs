using System;
using System.Threading;

namespace Rrs.Tasks
{
    public static class Schedule
    {
        public static void In(Action a, TimeSpan timespan, CancellationToken token)
        {
            Timer timer = null;
            timer = new Timer(_ =>
            {
                timer.Dispose();
                if (token.IsCancellationRequested) return;
                a();
            });
            
            timer.Change(Math.Max((int)timespan.TotalMilliseconds, 0), Timeout.Infinite);
        }

        public static ScheduledOperation In(Action a, TimeSpan timespan)
        {
            ScheduledOperation scheduled = null;
            Timer timer = null;
            timer = new Timer(_ =>
            {
                scheduled.Cancel();
                a();
            });
            scheduled = new ScheduledOperation(timer);

            timer.Change(Math.Max((int)timespan.TotalMilliseconds, 0), Timeout.Infinite);

            return scheduled;
        }
    }
}

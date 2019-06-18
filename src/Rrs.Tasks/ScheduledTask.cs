using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rrs.Tasks
{
    public class ScheduledTask
    {
        private readonly Action _action;
        private volatile ScheduledOperation _scheduled;

        public ScheduledTask(Action action)
        {
            _action = action;
        }

        public void ScheduleIn(TimeSpan timeSpan)
        {
            var scheduled = Interlocked.Exchange(ref _scheduled, Schedule.In(_action, timeSpan));
            scheduled?.Cancel();
        }

        public void ScheduleIn(DateTime datetime)
        {
            var timeSpan = datetime - DateTime.Now;
            var scheduled = Interlocked.Exchange(ref _scheduled, Schedule.In(_action, timeSpan));
            scheduled?.Cancel();
        }

        public void Cancel()
        {
            _scheduled?.Cancel();
        }
    }
}

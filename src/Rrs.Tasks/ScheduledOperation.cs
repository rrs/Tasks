using System.Diagnostics;
using System.Threading;

namespace Rrs.Tasks
{
    public class ScheduledOperation
    {
        private volatile int _cancelled;
        private readonly Timer _timer;

        internal ScheduledOperation(Timer timer)
        {
            _timer = timer;
        }

        public void Cancel()
        {
            if (Interlocked.Exchange(ref _cancelled, 1) == 1) return;

            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
        }

    }
}

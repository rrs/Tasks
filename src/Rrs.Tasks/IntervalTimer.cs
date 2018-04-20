using Rrs.DateTimes;
using System;
using System.Threading;

namespace Rrs.Tasks
{
    public class IntervalTimer
    {
        private readonly IRepeat _r;
        private Timer _t;
        private volatile bool _cancelled;

        private readonly ManualResetEvent _runningEvent = new ManualResetEvent(true);

        // allows us to block callers if the OnRepeat of the IRepeat is running
        public void Wait() => _runningEvent.WaitOne();

        public IntervalTimer(IRepeat r)
        {
            _r = r;
        }

        public IntervalTimer(TimeSpan rate, Action a) : this(new RepeatedAction(rate, a)) { }

        public IntervalTimer Start()
        {
            if (_t == null)
            {
                _t = new Timer(_ => Execute(), null, 0, Timeout.Infinite); // instant callback
            }
            return this;
        }

        public void Cancel()
        {
            if (_cancelled) return;
            _cancelled = true;
            _t.Change(Timeout.Infinite, Timeout.Infinite);
            _t.Dispose();
        }

        private void Execute()
        {
            if (_cancelled) return;
            _runningEvent.Reset();  // set the gate to closed
            _r.OnRepeat();          // run the timed task
            _runningEvent.Set();    // set the gate to open
            if (_cancelled) return;
            var now = DateTime.Now; // keep ref to now to prevent 'overshoot'
            var delay = now.RoundUp(_r.Rate) - now; // get the next interval less now to give how long we need to wait
            _t.Change((int)delay.TotalMilliseconds, Timeout.Infinite); // set timer going again
        }
    }
}

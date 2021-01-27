using Rrs.DateTimes;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks
{
    public class IntervalTimer
    {
        public static event Action<Exception> IntervalTimerException;

        private readonly IRepeatAsync _r;
        private Timer _t;
        private volatile bool _cancelled;

        private readonly ManualResetEvent _runningEvent = new ManualResetEvent(true);

        // allows us to block callers if the OnRepeat of the IRepeat is running
        public void Wait() => _runningEvent.WaitOne();

        public Task WaitAsync() => _runningEvent.WaitOneAsync();

        public IntervalTimer(IRepeat r)
        {
            _r = new RepeatedActionWrapper(r);
        }

        public IntervalTimer(IRepeatAsync r)
        {
            _r = r;
        }


        public IntervalTimer(TimeSpan rate, Action a) : this(new RepeatedAction(rate, a)) { }
        public IntervalTimer(TimeSpan rate, Func<Task> f) : this(new RepeatedActionAsync(rate, f)) { }

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
            try
            {
                var next = DateTime.Now.RoundToNearest(_r.Rate).Add(_r.Rate); // calculate the next desired time
                if (_cancelled) return;
                _runningEvent.Reset();  // set the gate to closed
                _r.OnRepeat().ContinueWith(t => 
                {
                    _runningEvent.Set();    // set the gate to open
                    if (_cancelled) return;

                    var now = DateTime.Now;
                    var delay = next - now; // work out the delay after the execution has happened, it may take a while
                    Debug.WriteLine(delay);
                    delay = delay.TotalMilliseconds > 0 ? delay : (now.RoundUp(_r.Rate) - now); // when OnRepeat takes longer than the next interval round to the next
                    _t.Change((int)delay.TotalMilliseconds, Timeout.Infinite); // set timer going again. 
                });          // run the timed task
                
            }
            catch(Exception e)
            {
                IntervalTimerException?.Invoke(new Exception("IntervalTimer exception", e));
            }
        }
    }
}

using Rrs.Tasks.Pulsable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks
{
    public sealed class IntervalPulseWorker : IDisposable
    {
        private PulseWorker _pulseWorker;
        private IntervalTimer _intervalTimer;

        public IntervalPulseWorker(PulseWorker pulseWorker, TimeSpan interval)
        {
            _pulseWorker = pulseWorker;
            _intervalTimer = new IntervalTimer(interval, () => _pulseWorker.Pulse());
        }

        public IntervalPulseWorker(IPulsable pulsable, TimeSpan interval) : this(new PulseWorker(pulsable), interval) { }

        public IntervalPulseWorker(Action<CancellationToken> pulseAction, TimeSpan interval) : this(new PulseWorker(pulseAction), interval) { }

        public IntervalPulseWorker(IAsyncPulsable pulsable, TimeSpan interval) : this(new PulseWorker(pulsable), interval) { }

        public IntervalPulseWorker(Func<CancellationToken, Task> pulseAction, TimeSpan interval) : this(new PulseWorker(pulseAction), interval) { }

        public void Pulse()
        {
            _pulseWorker.Pulse();
        }

        public void Dispose()
        {
            _intervalTimer.Cancel();
            _pulseWorker.Dispose();
        }
    }
}

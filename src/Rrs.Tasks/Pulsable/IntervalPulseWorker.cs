using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable;

public class IntervalPulseWorker : IPulseWorker, IDisposable
{
    private readonly PulseWorker _pulseWorker;
    private readonly IntervalTimer _timer;

    private IntervalPulseWorker(TimeSpan timeSpan, PulseWorker pulseWorker)
    {
        _pulseWorker = pulseWorker;
        _timer = new IntervalTimer(timeSpan, _pulseWorker.Pulse);
    }

    public IntervalPulseWorker(IPulsable pulsable, TimeSpan timeSpan) : this(timeSpan, new PulseWorker(pulsable)) { }

    public IntervalPulseWorker(Action<CancellationToken> pulseAction, TimeSpan timeSpan) : this(timeSpan, new PulseWorker(pulseAction)) { }

    public IntervalPulseWorker(IAsyncPulsable pulsable, TimeSpan timeSpan) : this(timeSpan, new PulseWorker(pulsable)) { }

    public IntervalPulseWorker(Func<CancellationToken, Task> pulseAction, TimeSpan timeSpan) : this(timeSpan, new PulseWorker(pulseAction)) { }

    public void Dispose()
    {
        _timer.Cancel();
        _pulseWorker.Dispose();
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Pulse()
    {
        _pulseWorker.Pulse();
    }
}

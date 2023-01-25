using Rrs.DateTimes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public class IntervalTimer : IDisposable
{
    public static event EventHandler<Exception> IntervalTimerException;

    private readonly IRepeatAsync _r;
    private Timer _t;
    private CancellationTokenSource _cts;

    private bool Cancelled => _cts.IsCancellationRequested;
    private readonly ManualResetEvent _runningEvent = new(true);

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

    public IntervalTimer(TimeSpan rate, Action<CancellationToken> a) : this(new RepeatedAction(rate, a)) { }
    public IntervalTimer(TimeSpan rate, Func<CancellationToken, Task> f) : this(new RepeatedActionAsync(rate, f)) { }

    public IntervalTimer Start()
    {
        if (_t == null)
        {
            _t = new Timer(_ => Execute(), null, 0, Timeout.Infinite); // instant callback
            _cts = new CancellationTokenSource();
        }
        else
        {
            _t.Change(0, Timeout.Infinite);
            var oldCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
            oldCts?.Dispose();
        }
        return this;
    }

    public void Stop()
    {
        _t?.Change(Timeout.Infinite, Timeout.Infinite);
        _cts?.Cancel();
    }

    private async void Execute()
    {
        try
        {
            var next = DateTime.Now.RoundToNearest(_r.Rate).Add(_r.Rate); // calculate the next desired time
            if (Cancelled) return;
            _runningEvent.Reset();  // set the gate to closed
            await _r.OnRepeat(_cts.Token);
            _runningEvent.Set();    // set the gate to open
            if (Cancelled) return;

            var now = DateTime.Now;
            var delay = next - now; // work out the delay after the execution has happened, it may take a while
            delay = delay.TotalMilliseconds > 0 ? delay : (now.RoundUp(_r.Rate) - now); // when OnRepeat takes longer than the next interval round to the next
            _t.Change((int)delay.TotalMilliseconds, Timeout.Infinite); // set timer going again. 

        }
        catch(Exception e)
        {
            IntervalTimerException?.Invoke(this, e);
        }
    }

    private bool _disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _t?.Dispose();
                _cts?.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

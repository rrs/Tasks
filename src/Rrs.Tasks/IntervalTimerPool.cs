using Rrs.DateTimes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public class IntervalTimerPool : IDisposable
{
    public static event EventHandler<Exception> IntervalTimerException;

    private Timer _t;
    private CancellationTokenSource _cts;
    private bool Cancelled => _cts.IsCancellationRequested;
    private readonly ManualResetEvent _runningEvent = new(true);

    private readonly ConcurrentDictionary<TimeSpan, RepeatList> _repeats = new();

    public IntervalTimerPool() { }

    public IntervalTimerPool(IEnumerable<RepeatList> repeats)
    {
        foreach(var r in repeats)
        {
            _repeats.TryAdd(r.Rate, r);
        }
    }

    public IntervalTimerPool(IEnumerable<(TimeSpan rate, Action<CancellationToken> a, int? priority)> repeats)
    {
        foreach (var (rate, a, p) in repeats)
        {
            this[rate].Add(a, p);
        }
    }

    public IntervalTimerPool(IEnumerable<(TimeSpan rate, Func<CancellationToken,Task> f, int? priority)> repeats)
    {
        foreach (var (rate, f, p) in repeats)
        {
            this[rate].Add(f, p);
        }
    }

    public RepeatList this[TimeSpan rate]
    {
        get => _repeats.GetOrAdd(rate, ts => new RepeatList(rate));
    }

    public IntervalTimerPool Start()
    {
        if (_t == null)
        {
            _t = new Timer(_ => Execute(), null, 0, Timeout.Infinite); // instant callback
            _nextRepeats = _repeats.Values.SelectMany(r => r).OrderBy(o => o.priority).Select(o => o.repeat);
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

    private IEnumerable<IDoAsync> _nextRepeats;

    private async void Execute()
    {
        try
        {
            var now = DateTime.Now;

            var next = DateTime.MaxValue;
            TimeSpan nextRate = TimeSpan.MaxValue; 
            foreach(var r in _repeats.Keys)
            {
                var n = now.RoundToNearest(r).Add(r);
                if (n < next)
                {
                    next = n;
                    nextRate = r;
                }
            }

            var nextRepeats = _repeats.Where(o => next.Ticks % o.Key.Ticks == 0).SelectMany(o => o.Value).OrderBy(o => o.priority).Select(o => o.repeat);
            if (Cancelled) return;
            _runningEvent.Reset(); // set the gate to closed
            foreach(var r in _nextRepeats)
            {
                if (Cancelled) return;
                await r.Do(_cts.Token);
            }
            _runningEvent.Set(); // set the gate to open
            if (Cancelled) return;
            _nextRepeats = nextRepeats;
            now = DateTime.Now;
            var delay = next - now; // work out the delay after the execution has happened, it may take a while
            delay = delay.TotalMilliseconds > 0 ? delay : TimeSpan.Zero; // when OnRepeat takes longer than the next interval fire straight away
            _t.Change((int)delay.TotalMilliseconds, Timeout.Infinite); // set timer going again. 
        }
        catch (Exception e)
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

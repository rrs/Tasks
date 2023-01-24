using Rrs.DateTimes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rrs.Tasks;

public class IntervalTimerPool : IDisposable
{
    public static event EventHandler<Exception> IntervalTimerException;

    private Timer _t;
    private volatile int _cancelled;
    private bool Cancelled => Interlocked.CompareExchange(ref _cancelled, 0, 0) == 1;
    private readonly ManualResetEvent _runningEvent = new(true);

    private readonly ConcurrentDictionary<TimeSpan, RepeatList> _repeats = new();

    public IntervalTimerPool() { }

    public IntervalTimerPool(IEnumerable<IRepeat> repeats)
    {
        foreach(var r in repeats)
        {
            this[r.Rate].Add(r);
        }
    }

    public IntervalTimerPool(IEnumerable<IRepeatAsync> repeats)
    {
        foreach (var r in repeats)
        {
            this[r.Rate].Add(r);
        }
    }

    public IntervalTimerPool(IEnumerable<(TimeSpan rate, Action a)> repeats)
    {
        foreach (var (rate, a) in repeats)
        {
            this[rate].Add(a);
        }
    }

    public IntervalTimerPool(IEnumerable<(TimeSpan rate, Func<Task> f)> repeats)
    {
        foreach (var (rate, f) in repeats)
        {
            this[rate].Add(f);
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
        }
        return this;
    }

    public void Cancel()
    {
        if (Interlocked.Exchange(ref _cancelled, 1) == 1) return;

        _t?.Change(Timeout.Infinite, Timeout.Infinite);
        _t?.Dispose();
    }

    private IEnumerable<IRepeatAsync> _nextRepeats;

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
                await r.OnRepeat();
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
                Cancel();
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

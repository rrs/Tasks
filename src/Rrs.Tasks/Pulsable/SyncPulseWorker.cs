using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable;

/// <summary>
/// Synchronous version of a pulse worker
/// </summary>
internal sealed class SyncPulseWorker : AbstractPulseWorker
{
    private readonly IPulsable _pulsable;

    public SyncPulseWorker(IPulsable pulsable, PulseWorkerOptions options = null) : base(options)
    {
        _pulsable = pulsable;
    }

    public SyncPulseWorker(Action<CancellationToken> pulseAction, PulseWorkerOptions options = null) : this(new SyncPulsableAdapter(pulseAction), options) { }
    
    protected override Task HandlePulse(CancellationToken token)
    {
        _pulsable.OnPulse(token);
        var tcs = new TaskCompletionSource<object>();
        tcs.SetResult(null);
        return tcs.Task;
    }
}

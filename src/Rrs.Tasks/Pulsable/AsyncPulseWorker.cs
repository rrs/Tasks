using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable;

/// <summary>
/// Asynchronous version of a pulse worker
/// </summary>
internal sealed class AsyncPulseWorker : AbstractPulseWorker
{
    private readonly IAsyncPulsable _pulsable;

    public AsyncPulseWorker(IAsyncPulsable pulsable, PulseWorkerOptions options = null) : base(options)
    {
        _pulsable = pulsable;
    }

    public AsyncPulseWorker(Func<CancellationToken, Task> pulseAction, PulseWorkerOptions options = null) : this(new AsyncPulsableAdapter(pulseAction), options) { }

    protected override Task HandlePulse(CancellationToken token)
    {
        return _pulsable.OnPulse(token);
    }
}

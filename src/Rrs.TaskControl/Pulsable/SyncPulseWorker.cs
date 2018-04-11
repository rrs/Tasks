using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.TaskControl.Pulsable
{
    /// <summary>
    /// Synchronous version of a pulse worker
    /// </summary>
    internal sealed class SyncPulseWorker : AbstractPulseWorker
    {
        private readonly IPulsable _pulsable;

        public SyncPulseWorker(IPulsable pulsable)
        {
            _pulsable = pulsable;
        }

        public SyncPulseWorker(Action<CancellationToken> pulseAction) : this(new SyncPulsableAdapter(pulseAction)) { }
        
        protected override Task HandlePulse(CancellationToken token)
        {
            _pulsable.OnPulse(token);
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}

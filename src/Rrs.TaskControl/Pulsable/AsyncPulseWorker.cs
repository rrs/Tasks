using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.TaskControl.Pulsable
{
    /// <summary>
    /// Asynchronous version of a pulse worker
    /// </summary>
    internal sealed class AsyncPulseWorker : AbstractPulseWorker
    {
        private readonly IAsyncPulsable _pulsable;

        public AsyncPulseWorker(IAsyncPulsable pulsable)
        {
            _pulsable = pulsable;
        }

        public AsyncPulseWorker(Func<CancellationToken, Task> pulseAction) : this(new AsyncPulsableAdapter(pulseAction)) { }

        protected override Task HandlePulse(CancellationToken token)
        {
            return _pulsable.OnPulse(token);
        }
    }
}

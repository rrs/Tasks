using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable
{
    public sealed class PulseWorker : IPulseWorker, IDisposable
    {
        private readonly AbstractPulseWorker _pulseWorker;

        public PulseWorker(IPulsable pulsable, PulseWorkerOptions options = null)
        {
            _pulseWorker = new SyncPulseWorker(pulsable, options);
        }

        public PulseWorker(Action<CancellationToken> pulseAction, PulseWorkerOptions options = null) : this(new SyncPulsableAdapter(pulseAction), options) { }

        public PulseWorker(IAsyncPulsable pulsable, PulseWorkerOptions options = null)
        {
            _pulseWorker = new AsyncPulseWorker(pulsable, options);
        }

        public PulseWorker(Func<CancellationToken, Task> pulseAction, PulseWorkerOptions options = null) : this(new AsyncPulsableAdapter(pulseAction), options) { }

        public void Dispose()
        {
            _pulseWorker.Dispose();
        }

        public void Pulse()
        {
            _pulseWorker.Pulse();
        }
    }
}

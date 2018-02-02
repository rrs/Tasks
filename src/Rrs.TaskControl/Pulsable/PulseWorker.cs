using System;
using System.Threading.Tasks;

namespace Rrs.TaskControl.Pulsable
{
    public sealed class PulseWorker : IPulseWorker, IDisposable
    {
        private readonly AbstractPulseWorker _pulseWorker;

        public PulseWorker(IPulsable pulsable)
        {
            _pulseWorker = new SyncPulseWorker(pulsable);
        }

        public PulseWorker(Action pulseAction) : this(new SyncPulsableAdapter(pulseAction)) { }

        public PulseWorker(IAsyncPulsable pulsable)
        {
            _pulseWorker = new AsyncPulseWorker(pulsable);
        }

        public PulseWorker(Func<Task> pulseAction) : this(new AsyncPulsableAdapter(pulseAction)) { }

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

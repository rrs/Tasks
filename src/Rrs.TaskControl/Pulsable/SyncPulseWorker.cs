using System;

namespace Rrs.TaskControl.Pulsable
{
    /// <summary>
    /// Synchronous version of a pulse worker
    /// </summary>
    public sealed class SyncPulseWorker : AbstractPulseWorker
    {
        private readonly IPulsable _pulsable;

        public SyncPulseWorker(IPulsable pulsable)
        {
            _pulsable = pulsable;
        }

        public SyncPulseWorker(Action pulseAction) : this(new SyncPulsableAdapter(pulseAction)) { }
        
        protected override void HandlePulse()
        {
            _pulsable.OnPulse();
            if (Disposed) return;
            RegisterWaitForPulse();
        }
    }
}

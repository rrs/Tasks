using System;

namespace Rrs.TaskControl.Pulsable
{
    /// <summary>
    /// Adapter class
    /// </summary>
    internal class SyncPulsableAdapter : IPulsable
    {
        private readonly Action _pulseAction;

        public SyncPulsableAdapter(Action pulseAction)
        {
            _pulseAction = pulseAction;
        }

        public void OnPulse()
        {
            _pulseAction();
        }
    }
}

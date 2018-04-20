using System;
using System.Threading;

namespace Rrs.Tasks.Pulsable
{
    /// <summary>
    /// Adapter class
    /// </summary>
    internal class SyncPulsableAdapter : IPulsable
    {
        private readonly Action<CancellationToken> _pulseAction;

        public SyncPulsableAdapter(Action<CancellationToken> pulseAction)
        {
            _pulseAction = pulseAction;
        }

        public void OnPulse(CancellationToken token)
        {
            _pulseAction(token);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable
{
    internal class AsyncPulsableAdapter : IAsyncPulsable
    {
        private readonly Func<CancellationToken, Task> _pulseAction;

        public AsyncPulsableAdapter(Func<CancellationToken, Task> pulseAction)
        {
            _pulseAction = pulseAction;
        }

        public Task OnPulse(CancellationToken token)
        {
            return _pulseAction(token);
        }
    }
}

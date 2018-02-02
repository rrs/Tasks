using System;
using System.Threading.Tasks;

namespace Rrs.TaskControl.Pulsable
{
    internal class AsyncPulsableAdapter : IAsyncPulsable
    {
        private readonly Func<Task> _pulseAction;

        public AsyncPulsableAdapter(Func<Task> pulseAction)
        {
            _pulseAction = pulseAction;
        }

        public Task OnPulse()
        {
            return _pulseAction();
        }
    }
}

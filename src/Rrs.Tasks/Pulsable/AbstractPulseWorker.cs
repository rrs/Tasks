using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable
{
    /// <summary>
    /// Core of the pulse worker logic. Provides the framework required to manage a single
    /// thread of execution running a piece of code.
    /// </summary>
    internal abstract class AbstractPulseWorker : IPulseWorker, IDisposable
    {
        private readonly AutoResetEvent _pulseEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private RegisteredWaitHandle _registeredWaitHandle;

        protected AbstractPulseWorker()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            RegisterWaitForPulse();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _pulseEvent.Dispose();
            _registeredWaitHandle.Unregister(_pulseEvent);
            _cancellationTokenSource.Dispose();
        }

        public void Pulse()
        {
            _pulseEvent.Set();
        }

        protected abstract Task HandlePulse(CancellationToken token);

        private void RegisterWaitForPulse()
        {
            if (_cancellationToken.IsCancellationRequested) return;

            WaitOrTimerCallback callback = delegate 
            {
                _registeredWaitHandle.Unregister(null);
                HandlePulse(_cancellationToken).ContinueWith(t => RegisterWaitForPulse());
            };

            _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_pulseEvent, callback, null, Timeout.Infinite, true);
        }
    }
}

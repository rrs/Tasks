using System;
using System.Threading;

namespace Rrs.TaskControl.Pulsable
{
    /// <summary>
    /// Core of the pulse worker logic. Provides the framework required to manage a single
    /// thread of execution running a piece of code.
    /// </summary>
    internal abstract class AbstractPulseWorker : IPulseWorker, IDisposable
    {
        private readonly AutoResetEvent _pulseEvent = new AutoResetEvent(false);
        private RegisteredWaitHandle _registeredWaitHandle;

        protected volatile bool Disposed;

        protected AbstractPulseWorker()
        {
            RegisterWaitForPulse();
        }

        public void Dispose()
        {
            Disposed = true;
            _pulseEvent.Dispose();
            _registeredWaitHandle.Unregister(_pulseEvent);
        }

        public void Pulse()
        {
            _pulseEvent.Set();
        }

        protected abstract void HandlePulse();

        protected void RegisterWaitForPulse()
        {
            _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_pulseEvent, (o, b) => HandlePulse(), null, Timeout.Infinite, true);
        }
    }
}

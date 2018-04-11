using System.Threading;

namespace Rrs.TaskControl.Pulsable
{
    public interface IPulsable
    {
        void OnPulse(CancellationToken token);
    }
}

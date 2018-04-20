using System.Threading;

namespace Rrs.Tasks.Pulsable
{
    public interface IPulsable
    {
        void OnPulse(CancellationToken token);
    }
}

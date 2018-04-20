using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks.Pulsable
{
    public interface IAsyncPulsable
    {
        Task OnPulse(CancellationToken token);
    }
}

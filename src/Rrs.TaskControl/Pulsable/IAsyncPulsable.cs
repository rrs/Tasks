using System.Threading;
using System.Threading.Tasks;

namespace Rrs.TaskControl.Pulsable
{
    public interface IAsyncPulsable
    {
        Task OnPulse(CancellationToken token);
    }
}

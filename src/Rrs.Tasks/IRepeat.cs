using System;
using System.Threading;

namespace Rrs.Tasks;

public interface IRepeat
{
    TimeSpan Rate { get; }
    void OnRepeat(CancellationToken cancellationToken);
}

using System;

namespace Rrs.Tasks;

public interface IRepeat
{
    TimeSpan Rate { get; }
    void OnRepeat();
}

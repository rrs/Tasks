using System;

namespace Rrs.TaskControl
{
    public interface IRepeat
    {
        TimeSpan Rate { get; }
        void OnRepeat();
    }
}

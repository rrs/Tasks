using System;

namespace Rrs.TaskControl
{
    class RepeatedAction : IRepeat
    {
        public TimeSpan Rate { get; }
        private readonly Action _a;
        public RepeatedAction(TimeSpan rate, Action a)
        {
            Rate = rate;
            _a = a;
        }

        public void OnRepeat()
        {
            _a();
        }
    }
}

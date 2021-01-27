using System;
using System.Threading.Tasks;

namespace Rrs.Tasks
{
    class RepeatedActionAsync : IRepeatAsync
    {
        public TimeSpan Rate { get; }

        private readonly Func<Task> _f;
        public RepeatedActionAsync(TimeSpan rate, Func<Task> f)
        {
            Rate = rate;
            _f = f;
        }

        public Task OnRepeat()
        {
            return _f();
        }
    }
}

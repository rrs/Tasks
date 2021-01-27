using System;
using System.Threading.Tasks;

namespace Rrs.Tasks
{
    class RepeatedActionWrapper : IRepeatAsync
    {
        private readonly IRepeat _r;
        public TimeSpan Rate => _r.Rate;

        public RepeatedActionWrapper(IRepeat r)
        {
            _r = r;
        }

        public Task OnRepeat()
        {
            _r.OnRepeat();
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}

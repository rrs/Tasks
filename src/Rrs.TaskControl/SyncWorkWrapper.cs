using System;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    class SyncWorkWrapper : IDoSomeWork
    {
        private readonly Action _a;
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        public Task Task => _tcs.Task;

        public SyncWorkWrapper(Action a)
        {
            _a = a;
        }

        public Task Execute()
        {
            _a();
            _tcs.SetResult(null);
            return _tcs.Task;
        }
    }

    public class SyncWorkWrapper<T> : IDoSomeWork
    {
        private readonly Func<T> _f;
        private readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();
        public Task<T> Task => _tcs.Task;

        public SyncWorkWrapper(Func<T> f)
        {
            _f = f;
        }

        public Task Execute()
        {
            var r = _f();
            _tcs.SetResult(r);
            return _tcs.Task;
        }
    }
}

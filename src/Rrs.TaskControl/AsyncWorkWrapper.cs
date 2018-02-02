using System;
using System.Threading.Tasks;

namespace Rrs.TaskControl
{
    class AsyncWorkWrapper : IDoSomeWork
    {
        private readonly Func<Task> _a;
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        public Task Task => _tcs.Task;

        public AsyncWorkWrapper(Func<Task> a)
        {
            _a = a;
        }

        public Task Execute()
        {
            _a().ContinueWith(t => _tcs.SetResult(null));
            return _tcs.Task;
        }
    }

    public class AsyncWorkWrapper<T> : IDoSomeWork
    {
        private readonly Func<Task<T>> _f;
        private readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();
        public Task<T> Task => _tcs.Task;

        public AsyncWorkWrapper(Func<Task<T>> f)
        {
            _f = f;
        }

        public Task Execute()
        {
            _f().ContinueWith(t => _tcs.SetResult(t.Result));
            return _tcs.Task;
        }
    }
}

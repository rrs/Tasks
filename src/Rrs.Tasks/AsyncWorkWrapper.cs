﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

internal class AsyncWorkWrapper : IDoSomeWork
{
    private readonly Func<CancellationToken, Task> _a;
    private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
    public Task Task => _tcs.Task;

    public AsyncWorkWrapper(Func<CancellationToken, Task> a)
    {
        _a = a;
    }

    public Task Execute(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            _tcs.SetCanceled();
        }
        else
        {
            _a(token).ContinueWith(t => _tcs.SetResult(null));
        }

        return _tcs.Task;
    }
}

internal class AsyncWorkWrapper<T> : IDoSomeWork
{
    private readonly Func<CancellationToken, Task<T>> _f;
    private readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();
    public Task<T> Task => _tcs.Task;

    public AsyncWorkWrapper(Func<CancellationToken, Task<T>> f)
    {
        _f = f;
    }

    public Task Execute(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            _tcs.SetCanceled();
        }
        else
        {
            _f(token).ContinueWith(t => _tcs.SetResult(t.Result));
        }
        
        return _tcs.Task;
    }
}

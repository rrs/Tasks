﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

internal class SyncWorkWrapper : IDoSomeWork
{
    private readonly Action<CancellationToken> _a;
    private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
    public Task Task => _tcs.Task;

    public SyncWorkWrapper(Action<CancellationToken> a)
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
            _a(token);
            _tcs.SetResult(null);
        }
        
        return _tcs.Task;
    }
}

internal class SyncWorkWrapper<T> : IDoSomeWork
{
    private readonly Func<CancellationToken, T> _f;
    private readonly TaskCompletionSource<T> _tcs = new TaskCompletionSource<T>();
    public Task<T> Task => _tcs.Task;

    public SyncWorkWrapper(Func<CancellationToken, T> f)
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
            var r = _f(token);
            _tcs.SetResult(r);
        }
        return _tcs.Task;
    }
}

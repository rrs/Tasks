﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rrs.Tasks;

public static class WaitHandleExtensions
{
    public static Task WaitOneAsync(this WaitHandle waitHandle)
    {
        if (waitHandle == null) throw new ArgumentNullException(nameof(waitHandle));

        var tcs = new TaskCompletionSource<bool>();
        var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle, delegate { tcs.TrySetResult(true); }, null, Timeout.Infinite, true);
        var t = tcs.Task;
        t.ContinueWith((antecedent) => rwh.Unregister(null));
        return t;
    }
}

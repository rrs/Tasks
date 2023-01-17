using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rrs.Tasks;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Rrs.Tasks;

[TestClass]
public class TaskQueueTests
{
    const int _delay = 20;
    const int _iterations = 50;

    [TestMethod]
    public async Task TestActionEnqueueMethod()
    {
        var queue = new TaskQueue();

        var colletion = new BlockingCollection<int>();

        var lastTask = Task.CompletedTask;
        foreach (var i in Enumerable.Range(0, _iterations))
        {
            var j = i;
            lastTask = queue.Enqueue(t =>
            {
                Thread.Sleep(_delay);
                colletion.TryAdd(j);
            });
        }

        await lastTask;

        ValidateCollection(colletion);
    }

    [TestMethod]
    public async Task TestFuncEnqueueMethod()
    {
        var queue = new TaskQueue();

        var colletion = new BlockingCollection<int>();

        var lastTask = Task.CompletedTask;

        foreach (var i in Enumerable.Range(0, _iterations))
        {
            var j = i;
            var task = queue.Enqueue(t =>
            {
                Thread.Sleep(_delay);
                return j;
            });

            lastTask = task.ContinueWith(t => colletion.TryAdd(t.Result));
        }

        await lastTask;

        ValidateCollection(colletion);
    }

    [TestMethod]
    public async Task TestTaskFuncEnqueueMethod()
    {
        var queue = new TaskQueue();

        var colletion = new BlockingCollection<int>();

        async Task addToQueue(int n)
        {
            await Task.Delay(_delay);
            colletion.Add(n);
        }

        var lastTask = Task.CompletedTask;

        foreach (var i in Enumerable.Range(0, _iterations))
        {
            var j = i;
            lastTask = queue.Enqueue(t => addToQueue(j));
        }

        await lastTask;

        ValidateCollection(colletion);
    }

    [TestMethod]
    public async Task TestTypedTaskFuncEnqueueMethod()
    {
        var queue = new TaskQueue();

        var colletion = new BlockingCollection<int>();

        async Task<int> addToQueue(int n)
        {
            await Task.Delay(_delay);
            return n;
        }

        var lastTask = Task.CompletedTask;

        foreach (var i in Enumerable.Range(0, _iterations))
        {
            var j = i;
            lastTask = queue.Enqueue(t => addToQueue(j)).ContinueWith(t => colletion.TryAdd(t.Result));
        }

        await lastTask;

        ValidateCollection(colletion);
    }

    private void ValidateCollection(BlockingCollection<int> c)
    {
        Assert.AreEqual(_iterations, c.Count);

        var v = 0;
        foreach (var i in c)
        {
            Assert.AreEqual(v, i);
            v++;
        }
    }
}

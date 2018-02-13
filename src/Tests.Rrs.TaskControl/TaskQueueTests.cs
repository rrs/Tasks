using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rrs.TaskControl;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Rrs.TaskControl
{
    [TestClass]
    public class TaskQueueTests
    {
        const int _delay = 10;
        const int _iterations = 50;

        [TestMethod]
        public async Task TestActionEnqueuMethod()
        {
            var queue = new TaskQueue();

            var colletion = new BlockingCollection<int>();

            var lastTask = Task.CompletedTask;
            foreach (var i in Enumerable.Range(0, _iterations))
            {
                lastTask = queue.Enqueue(() =>
                {
                    colletion.TryAdd(i);
                    Thread.Sleep(_delay);
                });
            }

            await lastTask;

            ValidateCollection(colletion);
        }

        [TestMethod]
        public async Task TestFuncEnqueuMethod()
        {
            var queue = new TaskQueue();

            var colletion = new BlockingCollection<int>();

            var lastTask = Task.CompletedTask;

            foreach (var i in Enumerable.Range(0, _iterations))
            {
                var task = queue.Enqueue(() =>
                {
                    Thread.Sleep(_delay);
                    return i;
                });

                lastTask = task.ContinueWith(t => colletion.TryAdd(t.Result));
            }

            await lastTask;

            ValidateCollection(colletion);
        }

        [TestMethod]
        public async Task TestTaskFuncEnqueuMethod()
        {
            var queue = new TaskQueue();

            var colletion = new BlockingCollection<int>();

            async Task addToQueue(int n)
            {
                colletion.Add(n);
                await Task.Delay(_delay);
            }

            var lastTask = Task.CompletedTask;

            foreach (var i in Enumerable.Range(0, _iterations))
            {
                lastTask = queue.Enqueue(() => addToQueue(i));
            }

            await lastTask;

            ValidateCollection(colletion);
        }

        [TestMethod]
        public async Task TestTypedTaskFuncEnqueuMethod()
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
                lastTask = queue.Enqueue(() => addToQueue(i)).ContinueWith(t => colletion.TryAdd(t.Result));
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

}

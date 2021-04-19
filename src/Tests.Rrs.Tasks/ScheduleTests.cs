using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rrs.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Rrs.Tasks
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public async Task TestScheduleIn()
        {
            var sw = new Stopwatch();
            Action a = sw.Stop;

            sw.Start();
            await Schedule.In(a, TimeSpan.FromSeconds(1));

            Assert.IsTrue(sw.ElapsedMilliseconds > 900);
        }
    }
}

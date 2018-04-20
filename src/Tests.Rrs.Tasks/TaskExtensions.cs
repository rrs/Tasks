using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Rrs.TaskControl
{
    static class TaskExtensions
    {
        public static void DoNotAwait(this Task _) {}
    }
}


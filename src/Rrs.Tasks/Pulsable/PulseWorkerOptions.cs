using System;
using System.Collections.Generic;
using System.Text;

namespace Rrs.Tasks.Pulsable
{
    public class PulseWorkerOptions
    {
        public PulseResetWhen ResetRelativeToAction { get; set; } = PulseResetWhen.Before;
    }
}

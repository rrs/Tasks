namespace Rrs.Tasks.Pulsable;

public class PulseWorkerOptions
{
    public PulseResetWhen ResetRelativeToAction { get; set; } = PulseResetWhen.Before;
}

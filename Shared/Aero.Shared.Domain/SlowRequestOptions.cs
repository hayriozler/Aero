namespace Aero.Shared.Domain;


public sealed class SlowRequestOptions
{
    public const string Section = "Diagnostics:SlowRequest";

    public int ThresholdMs { get; set; }
    public TimeSpan Threshold { get; }
}

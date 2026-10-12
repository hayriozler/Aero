namespace Infrastructure.Diagnostics;

public sealed class SlowRequestOptions
{
    public const string Section = "Diagnostics:SlowRequest";
    public int ThresholdMs { get; set; } = 500;
    public TimeSpan Threshold => TimeSpan.FromMilliseconds(ThresholdMs);
}

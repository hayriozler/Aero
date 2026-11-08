namespace Aero.TextMessageParser.Api.Services;

public sealed class MessageProcessingOptions
{
    public const string Section = "MessageProcessing";

    public int BatchSize { get; set; } = 50;
    public int MaxParallelism { get; set; } = 4;
    public int MaxAttempts { get; set; } = 3;
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan StaleThreshold { get; set; } = TimeSpan.FromMinutes(2);
    public TimeSpan RecoveryInterval { get; set; } = TimeSpan.FromMinutes(1);
}

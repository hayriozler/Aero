using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class CounterStatus : IResourceStatus
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsOpenValue { get; private set; }
    public bool IsCloseValue { get; private set; }

    private CounterStatus() { }

}

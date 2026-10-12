namespace AeroFlight.Domain.Metadatas;

public sealed class GateType
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private GateType() { }
}

namespace AeroFlight.Domain.Metadatas;

public sealed class AircraftNoiseCategory
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private AircraftNoiseCategory() { }
}

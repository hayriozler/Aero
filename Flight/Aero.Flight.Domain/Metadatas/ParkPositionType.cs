namespace Aero.Flights.Domain.Metadatas;

public sealed class ParkPositionType
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private ParkPositionType() { }
}

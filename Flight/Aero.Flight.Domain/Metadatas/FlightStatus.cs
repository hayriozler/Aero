namespace Aero.Flights.Domain.Metadatas;

public sealed class FlightStatus
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsUpdatable { get; private set; }
    private FlightStatus() { }

}

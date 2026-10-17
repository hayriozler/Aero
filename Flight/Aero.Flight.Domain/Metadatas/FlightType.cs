namespace Aero.Flights.Domain.Metadatas;

public sealed class FlightType
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? DisplayCode { get; private set; }
    private FlightType() { }
}

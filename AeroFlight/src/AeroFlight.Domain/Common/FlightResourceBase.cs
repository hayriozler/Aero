namespace AeroFlight.Domain.Common;

public abstract class FlightResourceBase
{
    public long Id { get; private set; }
    public long FlightId { get; private set; }

    protected FlightResourceBase() { }

    protected FlightResourceBase(long flightId) => FlightId = flightId;
}

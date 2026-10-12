using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Ops.Resources;

public abstract class OpsCodeShare: FlightResourceBase
{
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;

    protected OpsCodeShare() { }

    protected OpsCodeShare(long flightId, string airlineCode, string flightNumber) : base(flightId)
    {
        AirlineCode = airlineCode;
        FlightNumber = flightNumber;
    }

    public void UpdateDesignator(string airlineCode, string flightNumber)
    {
        AirlineCode = airlineCode;
        FlightNumber = flightNumber;
    }
}

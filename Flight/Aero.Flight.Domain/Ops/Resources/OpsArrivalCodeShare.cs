namespace Aero.Flights.Domain.Ops.Resources;

public sealed class OpsArrivalCodeShare : OpsCodeShare
{
    private OpsArrivalCodeShare() { }

    private OpsArrivalCodeShare(long flightId, string airlineCode, string flightNumber)
        : base(flightId, airlineCode, flightNumber) { }

    public static OpsArrivalCodeShare Create(long flightId, string airlineCode, string flightNumber)
        => new(flightId, airlineCode, flightNumber);
}

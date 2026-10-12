namespace AeroFlight.Domain.Ops.Routing;

public sealed class OpsArrivalRouting : OpsRouting
{
    private OpsArrivalRouting() { }

    private OpsArrivalRouting(long flightId, string airportCode, int legNumber)
        : base(flightId, airportCode, legNumber) { }

    public static OpsArrivalRouting Create(long flightId, string airportCode, int legNumber)
        => new(flightId, airportCode, legNumber);
}

namespace AeroFlight.Domain.Ops.Routing;

public sealed class OpsDepartureRouting : OpsRouting
{
    private OpsDepartureRouting() { }

    private OpsDepartureRouting(long flightId, string airportCode, int legNumber)
        : base(flightId, airportCode, legNumber) { }

    public static OpsDepartureRouting Create(long flightId, string airportCode, int legNumber)
        => new(flightId, airportCode, legNumber);
}

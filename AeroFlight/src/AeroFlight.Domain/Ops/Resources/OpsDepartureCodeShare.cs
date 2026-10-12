namespace AeroFlight.Domain.Ops.Resources;

public sealed class OpsDepartureCodeShare : OpsCodeShare
{
    private OpsDepartureCodeShare() { }

    private OpsDepartureCodeShare(long flightId, string airlineCode, string flightNumber)
        : base(flightId, airlineCode, flightNumber) { }

    public static OpsDepartureCodeShare Create(long flightId, string airlineCode, string flightNumber)
        => new(flightId, airlineCode, flightNumber);
}

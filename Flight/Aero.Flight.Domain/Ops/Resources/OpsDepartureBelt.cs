namespace Aero.Flights.Domain.Ops.Resources;

public sealed class OpsDepartureBelt : ResourceBase
{
    private OpsDepartureBelt() { }

    private OpsDepartureBelt(long flightId, string? code) : base(flightId, code) { }

    public static OpsDepartureBelt Create(long flightId, string? code = null) => new(flightId, code);
}

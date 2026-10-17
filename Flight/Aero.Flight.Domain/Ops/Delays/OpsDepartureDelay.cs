using Aero.Flights.Domain.Metadatas;

namespace Aero.Flights.Domain.Ops.Delays;

public sealed class OpsDepartureDelay : Delay
{
    private OpsDepartureDelay() { }

    private OpsDepartureDelay(long flightId, string delayCode, decimal? delayMinutes, string? comment)
        : base(flightId, delayCode, delayMinutes, comment) { }

    public static OpsDepartureDelay Create(long flightId, string delayCode, decimal? delayMinutes = null, string? comment = null)
        => new(flightId, delayCode, delayMinutes, comment);
}

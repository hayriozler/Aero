using AeroFlight.Domain.Metadatas;

namespace AeroFlight.Domain.Ops.Delays;

public sealed class OpsArrivalDelay : Delay
{
    private OpsArrivalDelay() { }

    private OpsArrivalDelay(long flightId, string delayCode, decimal? delayMinutes, string? comment)
        : base(flightId, delayCode, delayMinutes, comment) { }

    public static OpsArrivalDelay Create(long flightId, string delayCode, decimal? delayMinutes = null, string? comment = null)
        => new(flightId, delayCode, delayMinutes, comment);
}

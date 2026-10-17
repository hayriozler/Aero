using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops.Delays;

namespace Aero.Flights.Application.Flights.Delays;

public interface IDelayFactory<TDelay> where TDelay : Delay
{
    TDelay Create(long flightId, string delayCode, decimal? delayMinutes, string? comment);
}

public sealed class ArrivalDelayFactory : IDelayFactory<OpsArrivalDelay>
{
    public OpsArrivalDelay Create(long flightId, string delayCode, decimal? delayMinutes, string? comment) =>
        OpsArrivalDelay.Create(flightId, delayCode, delayMinutes, comment);
}

public sealed class DepartureDelayFactory : IDelayFactory<OpsDepartureDelay>
{
    public OpsDepartureDelay Create(long flightId, string delayCode, decimal? delayMinutes, string? comment) =>
        OpsDepartureDelay.Create(flightId, delayCode, delayMinutes, comment);
}

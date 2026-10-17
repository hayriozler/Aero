namespace Aero.Flights.Domain.Ops.Services;

public sealed class OpsDepartureService : OpsService
{
    private OpsDepartureService() { }

    private OpsDepartureService(long flightId, string serviceCode) : base(flightId, serviceCode) { }

    public static OpsDepartureService Create(long flightId, string serviceCode) => new(flightId, serviceCode);
}

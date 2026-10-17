namespace Aero.Flights.Domain.Ops.Services;

public sealed class OpsArrivalService : OpsService
{
    private OpsArrivalService() { }

    private OpsArrivalService(long flightId, string serviceCode) : base(flightId, serviceCode) { }

    public static OpsArrivalService Create(long flightId, string serviceCode) => new(flightId, serviceCode);
}

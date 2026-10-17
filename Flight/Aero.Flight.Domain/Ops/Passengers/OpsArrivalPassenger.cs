namespace Aero.Flights.Domain.Ops.Passengers;

public sealed class OpsArrivalPassenger : OpsPassenger
{
    private OpsArrivalPassenger() { }

    private OpsArrivalPassenger(long flightId, string? passengerType, int count)
        : base(flightId, passengerType, count) { }

    public static OpsArrivalPassenger Create(long flightId, string? passengerType, int count)
        => new(flightId, passengerType, count);
}

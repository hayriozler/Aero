namespace Aero.Flights.Domain.Ops.Passengers;

public sealed class OpsDeparturePassenger : OpsPassenger
{
    private OpsDeparturePassenger() { }

    private OpsDeparturePassenger(long flightId, string? passengerType, int count)
        : base(flightId, passengerType, count) { }

    public static OpsDeparturePassenger Create(long flightId, string? passengerType, int count)
        => new(flightId, passengerType, count);
}

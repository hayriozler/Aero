namespace Aero.Flights.Domain.Ops.Resources;

public sealed class OpsArrivalGate : ResourceBase
{
    private OpsArrivalGate() { }

    private OpsArrivalGate(long flightId, string? code) : base(flightId, code) { }

    public static OpsArrivalGate Create(long flightId, string? code = null) => new(flightId, code);
}

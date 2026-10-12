namespace AeroFlight.Domain.Ops.Resources;

public sealed class OpsDepartureGate : ResourceBase
{
    public DateTimeOffset? BoardingPlan { get; private set; }
    public DateTimeOffset? BoardingActual { get; private set; }

    private OpsDepartureGate() { }

    private OpsDepartureGate(long flightId, string? code) : base(flightId, code) { }

    public static OpsDepartureGate Create(long flightId, string? code = null) => new(flightId, code);

    public void SetBoardingTimes(DateTimeOffset? boardingPlan, DateTimeOffset? boardingActual = null)
    {
        BoardingPlan = boardingPlan;
        BoardingActual = boardingActual;
    }
}

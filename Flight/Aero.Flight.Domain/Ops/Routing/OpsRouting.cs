using Aero.Flights.Domain.Common;

namespace Aero.Flights.Domain.Ops.Routing;

public abstract class OpsRouting : FlightResourceBase
{
    public string Code { get; private set; } = string.Empty;
    public int LegNumber { get; private set; }
    public DateTimeOffset? Sta { get; private set; }
    public DateTimeOffset? Std { get; private set; }
    public DateTimeOffset? OnBlock { get; private set; }
    public DateTimeOffset? OffBlock { get; private set; }

    protected OpsRouting() { }

    protected OpsRouting(long flightId, string code, int legNumber)
        : base(flightId)
    {
        Code = code;
        LegNumber = legNumber;
    }

    public void UpdateTimes(DateTimeOffset? sta, DateTimeOffset? std,
        DateTimeOffset? onBlock = null, DateTimeOffset? offBlock = null)
    {
        Sta = sta;
        Std = std;
        OnBlock = onBlock;
        OffBlock = offBlock;
    }

    public void UpdateLeg(string code, int legNumber)
    {
        Code = code;
        LegNumber = legNumber;
    }
}

namespace Aero.Flights.Domain.Ops.Resources;

public sealed class OpsCounter : ResourceBase
{
    public string? CheckinClassCode { get; private set; }

    private OpsCounter() { }

    private OpsCounter(long flightId, string? code) : base(flightId, code) { }

    public static OpsCounter Create(long flightId, string? code = null) => new(flightId, code);

    public void UpdateCheckin(string? checkinClassCode) => CheckinClassCode = checkinClassCode;
}

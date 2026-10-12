namespace AeroFlight.Domain.Ops.Resources;

public sealed class OpsArrivalBelt : ResourceBase
{
    public DateTimeOffset? LastBag { get; private set; }

    private OpsArrivalBelt() { }

    private OpsArrivalBelt(long flightId, string? code) : base(flightId, code) { }

    public static OpsArrivalBelt Create(long flightId, string? code = null) => new(flightId, code);

    public void RecordLastBag(DateTimeOffset? lastBag) => LastBag = lastBag;
}

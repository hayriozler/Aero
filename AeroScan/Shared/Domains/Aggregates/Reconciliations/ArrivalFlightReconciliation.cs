using Domain.Common;

namespace Domain.Aggregates.Reconciliations;

public sealed class ArrivalFlightReconciliation : Entity<int>
{
    public int      FlightId               { get; private set; }
    public DateTime SnapshotAt                    { get; private set; }
    public bool     IsFinal                       { get; private set; }

    public int ExpectedBagCount            { get; private set; }
    public int UnloadedBagCount            { get; private set; }
    public int RemainingOnAircraftBagCount { get; private set; }
    public int ToBeltBagCount              { get; private set; }
    public int DeliveredBagCount           { get; private set; }
    public int TransferBagCount            { get; private set; }
    public int MissingBagCount             { get; private set; }
    public int UnknownBagCount             { get; private set; }
    public int RushBagCount                { get; private set; }

    private ArrivalFlightReconciliation() { }

    public static ArrivalFlightReconciliation Create(int flightId) => new()
    {
        FlightId = flightId,
        SnapshotAt      = DateTime.UtcNow
    };

    public void UpdateStats(
        int expected, int unloaded, int remaining, int toBelt,
        int delivered, int transfer, int missing, int unknown, int rush)
    {
        ExpectedBagCount            = expected;
        UnloadedBagCount            = unloaded;
        RemainingOnAircraftBagCount = remaining;
        ToBeltBagCount              = toBelt;
        DeliveredBagCount           = delivered;
        TransferBagCount            = transfer;
        MissingBagCount             = missing;
        UnknownBagCount             = unknown;
        RushBagCount                = rush;
        SnapshotAt                         = DateTime.UtcNow;
    }

    public void MarkFinal() => IsFinal = true;
}

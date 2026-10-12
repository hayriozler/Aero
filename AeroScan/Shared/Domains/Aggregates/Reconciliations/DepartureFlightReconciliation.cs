using Domain.Common;

namespace Domain.Aggregates.Reconciliations;

public sealed class DepartureFlightReconciliation : Entity<int>
{
    public int      FlightId          { get; private set; }
    public DateTime SnapshotAt                 { get; private set; }
    public bool     IsFinal                    { get; private set; }

    public int ExpectedBagCount            { get; private set; }
    public int LoadedBagCount              { get; private set; }
    public int OffloadedCount              { get; private set; }
    public int ToBeOffloadedCount          { get; private set; }
    public int WaitingToLoadBagCount       { get; private set; }
    public int MissingBagCount             { get; private set; }
    public int ReconciledBagCount          { get; private set; }
    public int ForceLoadedBagCount         { get; private set; }
    public int OnwardBagCount              { get; private set; }
    public int TransferLoadedBagCount      { get; private set; }
    public int TransferMissingBagCount     { get; private set; }
    public int NotBoardedPassengerBagCount { get; private set; }
    public int RushBagCount                { get; private set; }
    public int PriorityBagCount            { get; private set; }

    private DepartureFlightReconciliation() { }

    public static DepartureFlightReconciliation Create(int flightId) => new()
    {
        FlightId = flightId,
        SnapshotAt        = DateTime.UtcNow
    };

    public void UpdateStats(
        int expected, int loaded, int offloaded, int toBeOffloaded,
        int waiting, int missing, int reconciled, int forceLoaded,
        int onward, int transferLoaded, int transferMissing,
        int notBoardedPassenger, int rush, int priority)
    {
        ExpectedBagCount            = expected;
        LoadedBagCount              = loaded;
        OffloadedCount              = offloaded;
        ToBeOffloadedCount          = toBeOffloaded;
        WaitingToLoadBagCount       = waiting;
        MissingBagCount             = missing;
        ReconciledBagCount          = reconciled;
        ForceLoadedBagCount         = forceLoaded;
        OnwardBagCount              = onward;
        TransferLoadedBagCount      = transferLoaded;
        TransferMissingBagCount     = transferMissing;
        NotBoardedPassengerBagCount = notBoardedPassenger;
        RushBagCount                = rush;
        PriorityBagCount            = priority;
        SnapshotAt                  = DateTime.UtcNow;
    }

    public void MarkFinal() => IsFinal = true;
}

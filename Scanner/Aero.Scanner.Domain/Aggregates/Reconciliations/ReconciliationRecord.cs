using Aero.Scanner.Domain.Enums;
using Aero.Scanner.Domain.ValueObjects;
using Aero.Shared.Domain;

namespace Aero.Scanner.Domain.Aggregates.Reconciliations;

/// <summary>
/// Reconciliation process record — tracks the open/close lifecycle and unmatched bag lists.
/// Flight bag statistics are stored in DepartureFlightReconciliation / ArrivalFlightReconciliation.
/// </summary>
public sealed class ReconciliationRecord : AuditableEntity<int>
{
    private readonly List<BagTagNumber> _unmatchedBags = [];
    private readonly List<BagTagNumber> _rushBags = [];
    private readonly List<BagTagNumber> _offloadedBags = [];

    public ReconciliationStatus Status { get; private set; }
    public int TotalChecked { get; private set; }
    public int TotalLoaded { get; private set; }
    public int TotalOffloaded { get; private set; }
    public int TotalTransferred { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }

    public IReadOnlyList<BagTagNumber> UnmatchedBags => _unmatchedBags.AsReadOnly();
    public IReadOnlyList<BagTagNumber> RushBags => _rushBags.AsReadOnly();
    public IReadOnlyList<BagTagNumber> OffloadedBags => _offloadedBags.AsReadOnly();

    // EF Core
    private ReconciliationRecord() { }

    public static ReconciliationRecord Open() => new() { Status = ReconciliationStatus.Open };

    public void RecordBagCheckedIn() => TotalChecked++;
    public void RecordBagLoaded() => TotalLoaded++;
    public void RecordBagOffloaded(BagTagNumber tag) { TotalOffloaded++; _offloadedBags.Add(tag); }
    public void RecordBagTransferred() => TotalTransferred++;

    public void AddUnmatched(BagTagNumber tag)
    {
        if (!_unmatchedBags.Contains(tag))
        {
            _unmatchedBags.Add(tag);
            Status = ReconciliationStatus.Discrepancy;
        }
    }

    public void AddRushBag(BagTagNumber tag)
    {
        if (!_rushBags.Contains(tag))
            _rushBags.Add(tag);
    }

    public Result Close(string operatorId)
    {
        if (Status == ReconciliationStatus.Closed)
            return Result.Failure("Reconciliation is already closed.");

        Status   = ReconciliationStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = operatorId;
        return ResultFactory.Success();
    }
}

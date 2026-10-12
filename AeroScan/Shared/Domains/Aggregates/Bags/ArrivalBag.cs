using Domain.Common;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Aggregates.Bags;

public sealed class ArrivalBag : Entity<long>, IBag
{
    private IReadOnlyList<IBagEvent> _bagHistory = [];
   
    // EF Core
    private ArrivalBag() { }
    public string? CheckedWeight { get; private set; }
    public BaggageClass Class { get; private set; }
    public int? ContainerId { get; private set; }
    public string? RushReason { get; private set; }
    public bool IsTransfer { get; private set; } = false;
    public ArrivalBaggageStatus ArrivalBaggageStatus { get; private set; } = ArrivalBaggageStatus.Waiting;
    public ArrivalFlightPassenger FlightPassenger { get; private set; } = default!;

    public int FlightPassengerId { get; private set; }

    public string TagNumber { get; private set; } = default!;

    public char SourceIndicator { get; private set; }

    public string? SourceAirportCode { get; private set; }

    public int? HhtId { get; private set; }
    public string? UserName { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; } = false;
    public void AddEvent(IBagEvent bagEvent) => _bagHistory = [.. _bagHistory, bagEvent];
    public IReadOnlyList<IBagEvent> BagEvents => _bagHistory;
    public void SetDelete() => IsDeleted = true;
    //public static ArrivalBag Create(
    //    string bagTag,
    //    BaggageClass baggageClass,
    //    int? passengerId = null,
    //    bool isTransfer = false)
    //{
    //    var bag = new DepartureBag
    //    {
    //        TagNumber = tagNumber,
    //        Weight = weight,
    //        Class = baggageClass,
    //        PassengerId = passengerId,
    //        IsOnward = isTransfer,
    //        Status = BagStatus.Created
    //    };

    //    return bag;
    //}

    //public Result CheckIn(string operatorId, string deviceId, Location location, DateTime timestamp)
    //{
    //    if (Status != BagStatus.Created)
    //        return Result.Failure($"Bag must be in Created status to check in, current status: {Status}");

    //    Status = BagStatus.CheckedIn;
    //    _bagHistory.Add(new BagEvent(TagNumber, ScanType.CheckIn, location, deviceId, operatorId, timestamp));
    //    return Result.Success();
    //}

    //public Result MarkSorted(string operatorId, string deviceId, Location location, DateTime timestamp)
    //{
    //    if (Status is not (BagStatus.Created or BagStatus.CheckedIn))
    //        return Result.Failure($"Cannot sort bag in status: {Status}");

    //    //var previous = Status;
    //    Status = BagStatus.Sorted;
    //    _bagHistory.Add(new BagEvent(TagNumber, ScanType.Sortation, location, deviceId, operatorId, timestamp));
    //    return Result.Success();
    //}

    //public Result MarkLoaded(string operatorId, string deviceId, Location location, DateTime timestamp)
    //{
    //    if (Status is not (BagStatus.CheckedIn or BagStatus.Sorted))
    //        return Result.Failure($"Cannot load bag in status: {Status}");

    //    //var previous = Status;
    //    Status = BagStatus.Loaded;
    //    _bagHistory.Add(new BagEvent(TagNumber, ScanType.Loading, location, deviceId, operatorId, timestamp));
    //    return Result.Success();
    //}

    //public Result Offload(OffloadReason reason, string operatorId, string deviceId, Location location, DateTime timestamp)
    //{
    //    if (Status == BagStatus.Offloaded)
    //        return Result.Failure("Bag is already offloaded.");

    //    //var previous = Status;
    //    Status = BagStatus.Offloaded;
    //    OffloadReason = reason;
    //    _bagHistory.Add(new BagEvent(TagNumber, ScanType.Offload, location, deviceId, operatorId, timestamp));
    //    return Result.Success();
    //}

    ////public Result Transfer(FlightKey toFlight)
    ////{
    ////    if (Status == BagStatus.Transferred)
    ////        return Result.Failure("Bag is already transferred.");

    ////    //var previous = Status;
    ////    Status = BagStatus.Transferred;
    ////    IsOnward = true;
    ////    InboundFlightKey = FlightKey;
    ////    FlightKey = toFlight;
    ////    UpdatedAt = DateTime.UtcNow;
    ////    return Result.Success();
    ////}

    //public void UpsertFromBsm(Weight weight, int? passengerId, BaggageClass baggageClass)
    //{
    //    Weight = weight;
    //    if (passengerId.HasValue) PassengerId = passengerId;
    //    Class = baggageClass;
    //}

}

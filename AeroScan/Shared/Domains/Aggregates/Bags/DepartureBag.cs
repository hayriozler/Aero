using Domain.Common;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Aggregates.Bags;
public sealed class DepartureBag : Entity<long>, IBag
{
    private IReadOnlyList<IBagEvent> _bagHistory = [];
    private DepartureBag() { }

    public static DepartureBag Create(
        int flightPassengerId,
        char? ackRequest,
        bool authorityToLoad,
        bool authorityToTransport,
        char sourceIndicator,
        string? sourceAirportCode,
        string bagTagNumber,
        string destination,
        bool isOnward,
        BaggageClass clazz) => new()
        {
            FlightPassengerId = flightPassengerId,
            AckRequest = ackRequest,
            AuthorityToLoad = authorityToLoad,
            AuthorityToTransport = authorityToTransport,
            SourceIndicator = sourceIndicator,
            SourceAirportCode = sourceAirportCode,
            TagNumber = bagTagNumber,
            Destination = destination,
            IsOnward = isOnward,            
            Class = clazz,
            UpdatedAt = DateTime.UtcNow
        };

    public bool AuthorityToLoad { get; private set; } = true;
    public char? AckRequest { get; private set; }
    public bool AuthorityToTransport { get; private set; } = true;
    public char? WeightIndicator { get; private set; }
    public string? CheckedWeight { get; private set; }
    public OffloadReason? OffloadReason { get; private set; }
    public BaggageClass Class { get; private set; }
    public int? ContainerId { get; private set; }
    public string? RushReason { get; private set; }
    public DepartureBaggageStatus DepartureBaggageStatus { get; private set; } = DepartureBaggageStatus.Waiting;
    public bool IsPassengerBoarded { get; set; } = false;
    public bool IsOnward { get; set; } = false;
    public string Destination { get; private set; } = string.Empty;
    public DepartureFlightPassenger FlightPassenger { get; private set; } = default!;

    public int FlightPassengerId { get; private set; }

    public string TagNumber { get; private set; } = default!;

    public char SourceIndicator { get; private set; }

    public string? SourceAirportCode { get; private set; }

    public int? HhtId { get; private set; }
    public string? UserName { get; private set; }
    public DateTime UpdatedAt { get; private set;  }

    public bool IsDeleted { get; private set; } = false;
    public void AddEvent(IBagEvent bagEvent) => _bagHistory = [.. _bagHistory, bagEvent];
    public IReadOnlyList<IBagEvent> BagEvents => _bagHistory;
    public void SetDelete() => IsDeleted = true;

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

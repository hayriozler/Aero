using Domain.Aggregates.Flights;
using Domain.Common;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Aggregates.Bags;
public sealed class DepartureFlightPassenger : Entity, IFlightPassenger
{
    private readonly List<IBag> _bags = [];
    public DepartureFlight Flight { get; private set; } = default!;
    public PassengerStatus PassengerStatus { get; private set; } = PassengerStatus.None;
    private DepartureFlightPassenger() { }
    public static DepartureFlightPassenger Create(int flightId, string passengerName,
        string? securityNumber,
        string? sequenceNumber,
        string destination,
        bool isTransfer,
        string? seat,
        PassengerStatus passengerStatus = PassengerStatus.None
        ) => new()
        {
            FlightId       = flightId,
            PassengerName  = passengerName,
            SecurityNumber = securityNumber,
            SequenceNumber = sequenceNumber,
            Destination    = destination,
            IsOnward = isTransfer,
            Seat           = seat,
            PassengerStatus = passengerStatus
        };

    public int FlightId { get; private set; }
    public string? SecurityNumber { get; private set; }
    public string? SequenceNumber { get; private set; }
    public string? Seat { get; private set; }
    public string PassengerName { get; private set; } = default!;
    public string Destination { get; private set; } = default!;
    public void AddBag(IBag bag) => _bags.Add(bag);
    public void AddRangeBag(IBag[] bags) => _bags.AddRange(bags);
    public IReadOnlyList<IBag> Bags => _bags.AsReadOnly();

    public int Id { get; private set ; }
    public bool IsOnward { get; private set; } = false;
}

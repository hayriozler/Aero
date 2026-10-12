using Domain.Aggregates.Flights;
using Domain.Common;
using Domain.Interfaces;

namespace Domain.Aggregates.Bags;

public sealed class ArrivalFlightPassenger : Entity, IFlightPassenger
{
    private readonly List<IBag> _bags = [];
    private ArrivalFlightPassenger() { }
    public ArrivalFlight Flight { get; private set; } = default!;
    public static ArrivalFlightPassenger Create(int flightId, string passengerName,
        string? securityNumber,
        string? sequenceNumber,
        string destination,
        bool isTransfer,
        string? seat
        ) => new()
        {
            FlightId       = flightId,
            PassengerName  = passengerName,
            SecurityNumber = securityNumber,
            SequenceNumber = sequenceNumber,
            Destination  = destination,
            IsTransfer = isTransfer,
            Seat           = seat,


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
    public int Id { get; private set; }

    public bool IsTransfer { get; private set; } = false;
}
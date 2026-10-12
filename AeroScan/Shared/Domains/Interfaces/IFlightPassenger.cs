namespace Domain.Interfaces;

public interface IFlightPassenger : IKey<int>
{
    int FlightId { get; }
    string? SecurityNumber { get; }
    string? SequenceNumber { get; }
    string? Seat { get; }
    string PassengerName { get; } 
    string Destination { get; }
    void AddBag(IBag bag);// => _bags.Add(bag);
    void AddRangeBag(IBag[] bags);// => _bags.AddRange(bags);
    IReadOnlyList<IBag> Bags { get; }//=> _bags.AsReadOnly();   
}

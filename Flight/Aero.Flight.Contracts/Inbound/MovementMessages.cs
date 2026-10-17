namespace Aero.Flights.Contracts.Inbound;

public sealed record MovementMessageReceived(
    long? Id,
    bool IsArrival,
    string FlightNumber,
    DateTimeOffset Scheduled,
    string MessageType,
    string Content,
    DateTimeOffset ReceivedAt);

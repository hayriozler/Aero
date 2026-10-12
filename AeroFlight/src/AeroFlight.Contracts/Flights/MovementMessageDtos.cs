namespace AeroFlight.Contracts.Flights;

public sealed record MovementMessageResponse(
    long Id,
    long FlightId,
    string MessageType,
    string Content,
    DateTimeOffset ReceivedAt);

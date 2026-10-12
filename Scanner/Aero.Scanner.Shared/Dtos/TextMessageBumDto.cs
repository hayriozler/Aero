namespace Aero.Scanner.Shared.Dtos;

public sealed record TextMessageBumDto(
    long                   Id,
    string                 AirlineCode,
    string                 FlightNumber,
    string                 FlightIataDate,
    IReadOnlyList<string>  BagTagNumbers);

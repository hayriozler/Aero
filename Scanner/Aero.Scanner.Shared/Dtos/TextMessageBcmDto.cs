namespace Aero.Scanner.Shared.Dtos;

public sealed record TextMessageBcmDto(
    long   Id,
    string SubType,
    char? ackRequest,
    char? sourceIndicator,
    string sourceAirport,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate);

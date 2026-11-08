namespace Aero.TextMessageParser.Api.Dtos;

public sealed record TextMessageBcmDto(
    long Id,
    string SubType,
    char? ackRequest,
    char? sourceIndicator,
    string sourceAirport,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate);

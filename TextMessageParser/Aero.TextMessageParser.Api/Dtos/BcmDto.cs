namespace Aero.TextMessageParser.Api.Dtos;

public sealed record BcmDto(
    long Id,
    string SubType,
    char? AckRequest,
    char? SourceIndicator,
    string SourceAirportCode,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate);

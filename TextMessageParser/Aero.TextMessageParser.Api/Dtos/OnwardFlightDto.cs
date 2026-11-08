namespace Aero.TextMessageParser.Api.Dtos;

public sealed record OnwardFlightDto(
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    string Destination,
    char? Class);

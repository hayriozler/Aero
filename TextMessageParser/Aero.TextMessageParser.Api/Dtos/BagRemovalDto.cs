namespace Aero.TextMessageParser.Api.Dtos;

public sealed record BagRemovalDto(
    long Id,
    char? AckRequest,
    char SourceIndicator,
    string SourceAirportCode,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    string Destination,
    List<string> BagTags);

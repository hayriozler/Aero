namespace Aero.TextMessageParser.Api.Requests;

public sealed record TextMessageBumRequest(
    long Id,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    IReadOnlyList<string> BagTagNumbers);
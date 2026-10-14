namespace Aero.Scanner.Shared.Requests.TextMessages;

public sealed record TextMessageBcmRequest(
    long Id,
    string SubType,
    char? ackRequest,
    char? sourceIndicator,
    string sourceAirport,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate);

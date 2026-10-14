namespace Aero.Scanner.Shared.Requests.TextMessages;

public sealed record TextMessageBumRequest(
    long Id,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    IReadOnlyList<string> BagTagNumbers);

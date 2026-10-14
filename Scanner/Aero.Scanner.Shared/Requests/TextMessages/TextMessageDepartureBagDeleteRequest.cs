namespace Aero.Scanner.Shared.Requests.TextMessages;

public sealed record TextMessageDepartureBagDeleteRequest(long messageId,
            char? ackRequest,
            char sourceIndicator, string sourceAirport,
            string airlineCode, string flightNumber, string flightIataDate, string destination,
            List<string> BagTags);

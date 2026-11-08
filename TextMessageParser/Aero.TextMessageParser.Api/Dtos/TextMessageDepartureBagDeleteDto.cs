namespace Aero.TextMessageParser.Api.Dtos;

public sealed record TextMessageDepartureBagDeleteDto(long messageId,
            char? ackRequest,
            char sourceIndicator, string sourceAirport,
            string airlineCode, string flightNumber, string flightIataDate, string destination,
            List<string> BagTags);
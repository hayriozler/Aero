using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

internal static class BsmMapper
{
    public static DepartureBagDto ToDepartureBag(long messageId, ElementResult[] elementResults, string messageName)
    {
        var fields = BsmFields.Read(elementResults);
        var sourceIndicator = RequireSourceIndicator(fields, messageName, messageId);
        return new DepartureBagDto(
            messageId,
            fields.AckRequest, fields.AuthorityToLoad, fields.AuthorityToTransport,
            sourceIndicator, fields.SourceAirport,
            fields.AirlineCode, fields.FlightNumber, fields.FlightIataDate, fields.Destination,
            fields.BagTagStatus, fields.BagTags,
            fields.Onwards,
            fields.PassengerStatus, fields.PassengerName, fields.PassengerSecurityNumber, fields.PassengerSequenceNumber, fields.PassengerClass,
            fields.SeatNumber, fields.BaggageWeight, fields.PassengerProfileStatus);
    }

    public static BagRemovalDto ToBagRemoval(long messageId, ElementResult[] elementResults, string messageName)
    {
        var fields = BsmFields.Read(elementResults);
        var sourceIndicator = RequireSourceIndicator(fields, messageName, messageId);
        return new BagRemovalDto(
            messageId,
            fields.AckRequest,
            sourceIndicator, fields.SourceAirport,
            fields.AirlineCode, fields.FlightNumber, fields.FlightIataDate, fields.Destination,
            fields.BagTags);
    }

    private static char RequireSourceIndicator(BsmFields fields, string messageName, long messageId)
        => fields.SourceIndicator
           ?? throw new InvalidOperationException($"{messageName} {messageId}: BaggageSourceElement (SourceIndicator) is required but missing");
}

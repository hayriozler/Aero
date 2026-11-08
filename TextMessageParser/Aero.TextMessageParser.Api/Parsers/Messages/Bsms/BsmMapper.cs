using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Requests;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

internal static class BsmMapper
{
    public static TextMessageDepartureBagRequest ToDepartureBag(long messageId, ElementResult[] elementResults)
    {
        var fields = BsmFields.Read(elementResults);
        var sourceIndicator = RequireSourceIndicator(fields, "BSM", messageId);
        return new TextMessageDepartureBagRequest(
            messageId,
            fields.AckRequest, fields.AuthorityToLoad, fields.AuthorityToTransport,
            sourceIndicator, fields.SourceAirport,
            fields.AirlineCode, fields.FlightNumber, fields.FlightIataDate, fields.Destination,
            fields.BagTagStatus, fields.BagTags,
            fields.Onwards,
            fields.PassengerStatus, fields.PassengerName, fields.PassengerSecurityNumber, fields.PassengerSequenceNumber, fields.PassengerClass,
            fields.SeatNumber, fields.BaggageWeight, fields.PassengerProfileStatus);
    }

    public static TextMessageDepartureBagChgDto ToDepartureBagChange(long messageId, ElementResult[] elementResults)
    {
        var fields = BsmFields.Read(elementResults);
        var sourceIndicator = RequireSourceIndicator(fields, "BSM CHG", messageId);
        return new TextMessageDepartureBagChgDto(
            messageId,
            fields.AckRequest, fields.AuthorityToLoad, fields.AuthorityToTransport,
            sourceIndicator, fields.SourceAirport,
            fields.AirlineCode, fields.FlightNumber, fields.FlightIataDate, fields.Destination,
            fields.BagTagStatus, fields.BagTags,
            fields.Onwards,
            fields.PassengerStatus, fields.PassengerName, fields.PassengerSecurityNumber, fields.PassengerSequenceNumber, fields.PassengerClass,
            fields.SeatNumber, fields.BaggageWeight, fields.PassengerProfileStatus);
    }

    public static TextMessageDepartureBagDeleteDto ToDepartureBagDelete(long messageId, ElementResult[] elementResults)
    {
        var fields = BsmFields.Read(elementResults);
        var sourceIndicator = RequireSourceIndicator(fields, "BSM DEL", messageId);
        return new TextMessageDepartureBagDeleteDto(
            messageId,
            fields.AckRequest,
            sourceIndicator, fields.SourceAirport,
            fields.AirlineCode, fields.FlightNumber, fields.FlightIataDate, fields.Destination,
            fields.BagTags);
    }

    private static char RequireSourceIndicator(BsmFields fields, string messageName, long messageId)
        => fields.SourceIndicator
           ?? throw new InvalidOperationException($"{messageName} {messageId}: ElementV (SourceIndicator) is required but missing");
}

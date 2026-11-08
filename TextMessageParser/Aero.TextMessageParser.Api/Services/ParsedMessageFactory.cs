using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Requests;

namespace Aero.TextMessageParser.Api.Services;

public static class ParsedMessageFactory
{
    public static ParsedMessage? Create(TextMessage message, object result) => result switch
    {
        DepartureBagDto r => FromDeparture(message, r.AckRequest, r.AuthorityToLoad, r.AuthorityToTransport,
            r.SourceIndicator, r.SourceAirportCode, r.AirlineCode, r.FlightNumber, r.FlightIataDate, r.Destination,
            r.BagTagStatus, r.BagTags, r.Onwards, r.PassengerStatus, r.PassengerName, r.PassengerSecurityNumber,
            r.PassengerSequenceNumber, r.PassengerClass, r.SeatNumber, r.BaggageWeight, r.PassengerProfileStatus),
        BagRemovalDto r => ParsedMessage.Create(message.Id, message.Header.RoutingKey, r.AckRequest,
            r.SourceIndicator, r.SourceAirportCode, r.AirlineCode, r.FlightNumber, r.FlightIataDate, r.Destination, r.BagTags),
        BcmDto r => ParsedMessage.Create(message.Id, message.Header.RoutingKey, r.AckRequest,
            r.SourceIndicator, r.SourceAirportCode, r.AirlineCode, r.FlightNumber, r.FlightIataDate, null, []),
        _ => null
    };

    private static ParsedMessage FromDeparture(
        TextMessage message, char? ackRequest, bool authorityToLoad, bool authorityToTransport, char sourceIndicator,
        string sourceAirport, string airlineCode, string flightNumber, string flightDate, string destination,
        bool bagTagStatus, List<string> bagTags, List<OnwardFlightDto> onwards, char passengerStatus,
        string passengerName, string? securityNumber, string sequenceNumber, char? passengerClass, string? seatNumber,
        BaggageWeightDto? weight, char? profileStatus)
    {
        var parsed = ParsedMessage.Create(message.Id, message.Header.RoutingKey, ackRequest, sourceIndicator,
            sourceAirport, airlineCode, flightNumber, flightDate, destination, bagTags);
        parsed.SetAuthorities(authorityToLoad, authorityToTransport, bagTagStatus);
        parsed.SetPassenger(NullIfDefault(passengerStatus), NullIfEmpty(passengerName), NullIfEmpty(sequenceNumber),
            NullIfEmpty(securityNumber), passengerClass, profileStatus, NullIfEmpty(seatNumber));
        if (weight is not null)
            parsed.SetWeight(weight.PiecesWeightIndicator, weight.NumberOfCheckedBag, weight.CheckedWeight,
                weight.UncheckedWeight, weight.Unit, weight.LengthOfBag, weight.WidthOfBag, weight.HeightOfBag,
                weight.BaggageTypeCode);
        foreach (var o in onwards)
            parsed.AddOnward(o.AirlineCode, o.FlightNumber, o.FlightIataDate, o.Destination, o.Class);
        return parsed;
    }

    private static char? NullIfDefault(char value) => value == default ? null : value;

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}

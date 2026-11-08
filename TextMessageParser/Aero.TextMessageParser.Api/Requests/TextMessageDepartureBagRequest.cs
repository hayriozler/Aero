using Aero.TextMessageParser.Api.Dtos;

namespace Aero.TextMessageParser.Api.Requests;

public sealed record TextMessageDepartureBagRequest(
    long Id,
    char? AckRequest,
    bool AuthorityToLoad,
    bool AuthorityToTransport,
    char SourceIndicator,
    string SourceAirportCode,
    string AirlineCode,
    string FlightNumber,
    string FlightIataDate,
    string Destination,
    bool BagTagStatus,
    List<string> BagTags,
    List<OnwardFlightDto> Onwards,
    char PassengerStatus,
    string PassengerName,
    string? PassengerSecurityNumber,
    string PassengerSequenceNumber,
    char? PassengerClass,
    string? SeatNumber);

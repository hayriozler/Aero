namespace Aero.TextMessageParser.Api.Responses;

public sealed record ParsedMessageResponse(
    long Id,
    long TextMessageId,
    string MessageType,
    char? AckRequest,
    char? SourceIndicator,
    string SourceAirport,
    string AirlineCode,
    string FlightNumber,
    string FlightDate,
    string? Destination,
    bool? AuthorityToLoad,
    bool? AuthorityToTransport,
    bool? BagTagStatus,
    List<string> BagTags,
    List<ParsedOnwardFlightResponse> Onwards,
    ParsedPassengerResponse? Passenger,
    ParsedBaggageWeightResponse? Weight,
    DateTime RecordDateTime);

public sealed record ParsedOnwardFlightResponse(
    string AirlineCode,
    string FlightNumber,
    string FlightDate,
    string Destination,
    char? Class);

public sealed record ParsedPassengerResponse(
    char? Status,
    string? Name,
    string? SequenceNumber,
    string? SecurityNumber,
    char? Class,
    char? ProfileStatus,
    string? SeatNumber);

public sealed record ParsedBaggageWeightResponse(
    string? Indicator,
    string? CheckedBagCount,
    string? CheckedWeight,
    string? UncheckedWeight,
    string? Unit,
    string? Length,
    string? Width,
    string? Height,
    string? BaggageTypeCode);

public sealed record ConsumedRequest(List<long> Ids);

public sealed record ConsumedResponse(int Updated);

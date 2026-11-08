namespace Aero.TextMessageParser.Api.Entities;

public sealed class ParsedMessage
{
    private ParsedMessage() { }

    public long Id { get; private set; }
    public long TextMessageId { get; private set; }
    public string MessageType { get; private set; } = null!;
    public char? AckRequest { get; private set; }
    public char? SourceIndicator { get; private set; }
    public string SourceAirport { get; private set; } = null!;
    public string AirlineCode { get; private set; } = null!;
    public string FlightNumber { get; private set; } = null!;
    public string FlightDate { get; private set; } = null!;
    public string? Destination { get; private set; }
    public bool? AuthorityToLoad { get; private set; }
    public bool? AuthorityToTransport { get; private set; }
    public bool? BagTagStatus { get; private set; }
    public char? PassengerStatus { get; private set; }
    public string? PassengerName { get; private set; }
    public string? PassengerSequenceNumber { get; private set; }
    public string? PassengerSecurityNumber { get; private set; }
    public char? PassengerClass { get; private set; }
    public char? PassengerProfileStatus { get; private set; }
    public string? SeatNumber { get; private set; }
    public string? WeightIndicator { get; private set; }
    public string? CheckedBagCount { get; private set; }
    public string? CheckedWeight { get; private set; }
    public string? UncheckedWeight { get; private set; }
    public string? WeightUnit { get; private set; }
    public string? BagLength { get; private set; }
    public string? BagWidth { get; private set; }
    public string? BagHeight { get; private set; }
    public string? BaggageTypeCode { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsConsumed { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
    public List<ParsedBagTag> BagTags { get; private set; } = [];
    public List<ParsedOnwardFlight> Onwards { get; private set; } = [];

    public static ParsedMessage Create(
        long textMessageId, string messageType, char? ackRequest, char? sourceIndicator, string sourceAirport,
        string airlineCode, string flightNumber, string flightDate, string? destination,
        IEnumerable<string> bagTags) =>
        new()
        {
            TextMessageId = textMessageId,
            MessageType = messageType,
            AckRequest = ackRequest,
            SourceIndicator = sourceIndicator,
            SourceAirport = sourceAirport,
            AirlineCode = airlineCode,
            FlightNumber = flightNumber,
            FlightDate = flightDate,
            Destination = destination,
            CreatedAtUtc = DateTime.UtcNow,
            BagTags = bagTags.Select(ParsedBagTag.Create).ToList()
        };

    public void MarkConsumed(DateTime consumedAtUtc)
    {
        IsConsumed = true;
        ConsumedAt = consumedAtUtc;
    }

    public void SetAuthorities(bool authorityToLoad, bool authorityToTransport, bool bagTagStatus)
    {
        AuthorityToLoad = authorityToLoad;
        AuthorityToTransport = authorityToTransport;
        BagTagStatus = bagTagStatus;
    }

    public void SetPassenger(
        char? status, string? name, string? sequenceNumber, string? securityNumber,
        char? passengerClass, char? profileStatus, string? seatNumber)
    {
        PassengerStatus = status;
        PassengerName = name;
        PassengerSequenceNumber = sequenceNumber;
        PassengerSecurityNumber = securityNumber;
        PassengerClass = passengerClass;
        PassengerProfileStatus = profileStatus;
        SeatNumber = seatNumber;
    }

    public void SetWeight(
        string indicator, string checkedBagCount, string checkedWeight, string uncheckedWeight,
        string unit, string length, string width, string height, string baggageTypeCode)
    {
        WeightIndicator = indicator;
        CheckedBagCount = checkedBagCount;
        CheckedWeight = checkedWeight;
        UncheckedWeight = uncheckedWeight;
        WeightUnit = unit;
        BagLength = length;
        BagWidth = width;
        BagHeight = height;
        BaggageTypeCode = baggageTypeCode;
    }

    public void AddOnward(string airlineCode, string flightNumber, string flightDate, string destination, char? flightClass) =>
        Onwards.Add(ParsedOnwardFlight.Create(airlineCode, flightNumber, flightDate, destination, flightClass));
}

public sealed class ParsedBagTag
{
    private ParsedBagTag() { }
    public long Id { get; private set; }
    public long ParsedMessageId { get; private set; }
    public string BagTag { get; private set; } = null!;
    public static ParsedBagTag Create(string bagTag) => new() { BagTag = bagTag };
}

public sealed class ParsedOnwardFlight
{
    private ParsedOnwardFlight() { }
    public long Id { get; private set; }
    public long ParsedMessageId { get; private set; }
    public string AirlineCode { get; private set; } = null!;
    public string FlightNumber { get; private set; } = null!;
    public string FlightDate { get; private set; } = null!;
    public string Destination { get; private set; } = null!;
    public char? Class { get; private set; }
    public static ParsedOnwardFlight Create(string airlineCode, string flightNumber, string flightDate, string destination, char? flightClass) =>
        new() { AirlineCode = airlineCode, FlightNumber = flightNumber, FlightDate = flightDate, Destination = destination, Class = flightClass };
}

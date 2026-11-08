using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

internal sealed class BsmFields
{
    public char? AckRequest { get; private set; }
    public char? SourceIndicator { get; private set; }
    public string SourceAirport { get; private set; } = string.Empty;
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;
    public string FlightIataDate { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public char? PassengerClass { get; private set; }
    public string PassengerName { get; private set; } = string.Empty;
    public string? PassengerSecurityNumber { get; private set; }
    public string PassengerSequenceNumber { get; private set; } = string.Empty;
    public char PassengerStatus { get; private set; }
    public char? PassengerProfileStatus { get; private set; }
    public string? SeatNumber { get; private set; }
    public bool AuthorityToLoad { get; private set; }
    public bool AuthorityToTransport { get; private set; }
    public bool BagTagStatus { get; private set; }
    public BaggageWeightDto? BaggageWeight { get; private set; }
    public List<OnwardFlightDto> Onwards { get; } = [];
    public List<string> BagTags { get; } = [];

    public static BsmFields Read(IEnumerable<ElementResult> elementResults)
    {
        var fields = new BsmFields();
        foreach (var result in elementResults)
        {
            switch (result.Element)
            {
                case OutboundFlightElement f:
                    fields.AirlineCode = f.AirlineCode;
                    fields.FlightNumber = f.FlightNumber;
                    fields.FlightIataDate = f.FlightDate;
                    fields.PassengerClass = f.Class;
                    fields.Destination = f.Destination;
                    break;
                case BagTagElement n:
                    AddBagTags(fields.BagTags, n);
                    break;
                case OnwardFlightElement o:
                    fields.Onwards.Add(new OnwardFlightDto(o.AirlineCode, o.FlightNumber, o.FlightDate, o.Destination, o.Class));
                    break;
                case PassengerNameElement p:
                    fields.PassengerName = $"{p.PassengerGivenName} {p.PassengerSurname}";
                    break;
                case PassengerStatusElement s:
                    fields.AuthorityToLoad = s.AuthorityToLoad == 'Y';
                    fields.SeatNumber = s.SeatNumber;
                    fields.PassengerStatus = s.PassengerStatus;
                    fields.PassengerSecurityNumber = s.SecurityNumber;
                    fields.PassengerSequenceNumber = s.SequenceNumber;
                    fields.PassengerProfileStatus = s.PassengerProfileStatus == default(char) ? null : s.PassengerProfileStatus;
                    fields.AuthorityToTransport = s.AuthorityToTransport == 'Y';
                    fields.BagTagStatus = s.BaggageTagStatus == 'A';
                    break;
                case BaggageSourceElement v:
                    fields.AckRequest = v.AckRequest;
                    fields.SourceIndicator = v.BaggageSourceIndicator;
                    fields.SourceAirport = v.AirportCode;
                    break;
                case BaggageWeightElement w:
                    fields.BaggageWeight = new BaggageWeightDto(
                        w.PiecesWeightIndicator, w.NumberOfCheckedBag, w.CheckedWeight,
                        w.UncheckedWeight, w.Unit, w.LengthOfBag,
                        w.WidthOfBag, w.HeightOfBag, w.BaggageTypeCode);
                    break;
            }
        }
        return fields;
    }

    private static void AddBagTags(List<string> bagTags, BagTagElement element)
    {
        var count = int.TryParse(element.ConsecutiveTags, out var consecutive) ? consecutive : 0;
        var firstSerial = int.Parse(element.TagSerial);
        for (var i = 0; i < count; i++)
        {
            var serial = (firstSerial + i) % Consts.BagTagSerialModulus;
            bagTags.Add($"{element.AirlinePrefix}{serial:D6}");
        }
    }
}

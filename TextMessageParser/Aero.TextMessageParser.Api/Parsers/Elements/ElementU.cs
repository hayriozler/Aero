using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementU(IElementValidator validator) : Element(Consts.U)
{
    public string StowageDeviceId { get; private set; } = string.Empty;
    public string AircraftCompartmentLocation { get; private set; } = string.Empty;
    public string TypeOfBaggageInContainer { get; private set; } = string.Empty;
    public char ClassOfTravel { get; private set; }
    public string DestinationAirport { get; private set; } = string.Empty;
    public string SealedContainerIndicator { get; private set; } = string.Empty;
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;
    public string ConnectionDepartureDate { get; private set; } = string.Empty;
    public string DestinationTransferAirport { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementU(validator);
        var parsedText = elementDetail.ParsedText;
        result.StowageDeviceId             = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.AircraftCompartmentLocation = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        result.TypeOfBaggageInContainer = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        result.ClassOfTravel = parsedText.Length > 4 ? parsedText[4].Span[0] : default;
        result.DestinationAirport = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        result.SealedContainerIndicator = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        if (parsedText.Length > 7)
        {
            int al = GetAirlineLen(parsedText[7].Span[2]);
            result.AirlineCode  = parsedText[7][..al].ToString();
            result.FlightNumber = parsedText[7][al..].ToString();
        }
        result.ConnectionDepartureDate = parsedText.Length > 8 ? parsedText[8].ToString() : string.Empty;
        result.DestinationTransferAirport = parsedText.Length > 9 ? parsedText[9].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
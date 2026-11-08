using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class OnwardFlightElement(IElementValidator validator) : Element(Consts.O)
{
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;
    public string FlightDate { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public char? Class { get; private set; }
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new OnwardFlightElement(validator);
        var parsedText = elementDetail.ParsedText;
        int airlineLen = GetAirlineLen(elementDetail.ParsedText[1].Span[2]);
        result.AirlineCode  = elementDetail.ParsedText[1][..airlineLen].ToString();
        result.FlightNumber = elementDetail.ParsedText[1][airlineLen..].ToString();
        result.FlightDate  = elementDetail.ParsedText[2].ToString();
        result.Destination = elementDetail.ParsedText[3].ToString();
        result.Class = parsedText.Length > 4 ? parsedText[4].Span[0] : default;
        return new(result, validationResult);
    }
}
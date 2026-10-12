

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementF(IElementValidator validator) : Element(Consts.F)
{
    public string AirlineCode { get; private set; } = string.Empty;
    public string FlightNumber { get; private set; } = string.Empty;
    public string FlightDate   { get; private set; } = string.Empty;
    public string Destination  { get; private set; } = string.Empty;
    public char? Class        { get; private set; }
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        int airlineLen = GetAirlineLen(elementDetail.ParsedText[1].Span[2]);
        AirlineCode  = elementDetail.ParsedText[1][..airlineLen].ToString();
        FlightNumber = elementDetail.ParsedText[1][airlineLen..].ToString();
        FlightDate  = elementDetail.ParsedText[2].ToString();
        Destination = elementDetail.ParsedText[3].ToString();
        Class = parsedText.Length > 4 ? parsedText[4].Span[0] : default;
        return new(this, validationResult);
    }
}


using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementH(IElementValidator validator) : Element(Consts.H)
{
    public string HandlingTerminal { get; private set; } = string.Empty;
    public string HandlingBayPier  { get; private set; } = string.Empty;
    public string HandlingArea     { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        HandlingTerminal = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        HandlingBayPier  = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        HandlingArea     = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementY(IElementValidator validator) : Element(Consts.Y)
{
    public string FrequentTravellerId { get; private set; } = string.Empty;
    public string TierId { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementY(validator);
        var parsedText = elementDetail.ParsedText;
        result.FrequentTravellerId = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.TierId              = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
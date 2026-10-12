
using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementY(IElementValidator validator) : Element(Consts.Y)
{
    public string FrequentTravellerId { get; private set; } = string.Empty;
    public string TierId              { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        FrequentTravellerId = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        TierId              = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
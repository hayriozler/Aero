

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementT(IElementValidator validator) : Element(Consts.T)
{
    public string BagTagPrinterId { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        BagTagPrinterId = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
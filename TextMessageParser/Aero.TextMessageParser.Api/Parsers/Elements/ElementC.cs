using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementC(IElementValidator validator) : Element(Consts.C)
{
    public string FreeText { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementC(validator);
        var parsedText = elementDetail.ParsedText;
        result.FreeText = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
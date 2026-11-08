using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementR(IElementValidator validator) : Element(Consts.R)
{
    public string FreeText { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementR(validator);
        var parsedText = elementDetail.ParsedText;
        result.FreeText = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementL(IElementValidator validator) : Element(Consts.L)
{
    public string PnrAddress { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementL(validator);
        var parsedText = elementDetail.ParsedText;
        result.PnrAddress = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementG(IElementValidator validator) : Element(Consts.G)
{
    public string EarliestDeliveryDateTime { get; private set; } = string.Empty;
    public string LatestDeliveryDateTime { get; private set; } = string.Empty;
    public string FreeText { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementG(validator);
        var parsedText = elementDetail.ParsedText;

        result.EarliestDeliveryDateTime = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.LatestDeliveryDateTime   = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        result.FreeText                 = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
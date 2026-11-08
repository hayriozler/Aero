using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementA(IElementValidator validator) : Element(Consts.A)
{
    public string SenderMsgRefNumber { get; private set; } = string.Empty;
    public char TypeBaggageStatusIndicator { get; private set; }
    public char AckStatus { get; private set; }
    public string FreeText { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementA(validator);
        var parsedText = elementDetail.ParsedText;
        result.SenderMsgRefNumber        = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.TypeBaggageStatusIndicator = parsedText.Length > 2 ? parsedText[2].Span[0] : default;
        result.AckStatus                 = parsedText.Length > 3 ? parsedText[3].Span[0] : default;
        result.FreeText                  = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementA(IElementValidator validator) : Element(Consts.A)
{
    public string SenderMsgRefNumber { get; private set; } = string.Empty;
    public char TypeBaggageStatusIndicator { get; private set; }
    public char AckStatus                 { get; private set; }
    public string FreeText                  { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        SenderMsgRefNumber        = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        TypeBaggageStatusIndicator = parsedText.Length > 2 ? parsedText[2].Span[0] : default;
        AckStatus                 = parsedText.Length > 3 ? parsedText[3].Span[0] : default;
        FreeText                  = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
using Aero.MessageProcessor.TextMessageProcessor.Contracts;

namespace Aero.MessageProcessor.TextMessageProcessor.Parsers.Elements;

public class ElementC(IElementValidator validator) : Element(Consts.C)
{
    public string FreeText { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        FreeText = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
using Aero.MessageProcessor.TextMessageProcessor.Contracts;

namespace Aero.MessageProcessor.TextMessageProcessor.Parsers.Elements;

public class ElementK(IElementValidator validator) : Element(Consts.K)
{
    public string PrinterId { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        PrinterId = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
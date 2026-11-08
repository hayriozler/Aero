using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementQ(IElementValidator validator) : Element(Consts.Q)
{
    public string LoadSequenceNumber { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementQ(validator);
        var parsedText = elementDetail.ParsedText;
        result.LoadSequenceNumber = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
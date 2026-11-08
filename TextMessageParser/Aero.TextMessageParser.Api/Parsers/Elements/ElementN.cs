using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementN(IElementValidator validator) : Element(Consts.N)
{
    public string AirlinePrefix { get; private set; } = string.Empty;
    public string TagSerial { get; private set; } = string.Empty;
    public string ConsecutiveTags { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementN(validator);
        var parsedText = elementDetail.ParsedText;
        var tag = parsedText[1];
        result.AirlinePrefix   = tag[..4].ToString();
        result.TagSerial       = tag.Slice(4, 6).ToString();
        result.ConsecutiveTags = tag.Slice(10, 3).ToString();
        return new(result, validationResult);
    }
}
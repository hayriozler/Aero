using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementE(IElementValidator validator) : Element(Consts.E)
{
    public string BagExceptionType { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementE(validator);
        result.BagExceptionType = elementDetail.Text.ToString();
        return new(result, validationResult);
    }
}
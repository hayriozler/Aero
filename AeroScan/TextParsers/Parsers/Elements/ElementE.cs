

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementE(IElementValidator validator) : Element(Consts.E)
{
    public string BagExceptionType { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        BagExceptionType= validationResult.IsValid ? elementDetail.Text.ToString() : string.Empty;
        return new(this, validationResult);
    }
}
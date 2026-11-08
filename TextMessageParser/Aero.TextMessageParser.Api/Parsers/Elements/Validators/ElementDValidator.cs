using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementDValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 20 || elementDetail.Length > 52)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementD length wrong");
        }
        return validationResult;
    }
}

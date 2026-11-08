using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementEValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 6 || elementDetail.Length > 9)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementE length wrong");
        }
        return validationResult;
    }
}

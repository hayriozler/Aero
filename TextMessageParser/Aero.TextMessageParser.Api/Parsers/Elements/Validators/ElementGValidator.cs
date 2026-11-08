using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementGValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 12 || elementDetail.Length > 151)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementG length wrong");
        }
        return validationResult;
    }
}

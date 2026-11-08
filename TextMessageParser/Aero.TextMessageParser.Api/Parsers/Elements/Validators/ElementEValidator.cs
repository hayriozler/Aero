using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementEValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 8 || elementDetail.Length > 9)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementE length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length != 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementE structure wrong");
            return validationResult;
        }
        foreach (var c in elementDetail.ParsedText[1].Span)
            if (!char.IsLetter(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "ElementE exception type must be letters");
                return validationResult;
            }
        return validationResult;
    }
}

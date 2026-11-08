using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementTValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 6 || elementDetail.Length > 12)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementT length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, "ElementT field missing");
            return validationResult;
        }
        var f = elementDetail.ParsedText[1];
        if (f.Length < 1 || f.Length > 7)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementT printer ID len wrong");
            return validationResult;
        }
        foreach (var c in f.Span)
            if (!char.IsLetterOrDigit(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "ElementT printer ID invalid char");
                return validationResult;
            }
        return validationResult;
    }
}

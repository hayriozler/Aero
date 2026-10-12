using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementTValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 6 || elementDetail.Length > 12)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementT length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementT field missing");
            return validationResult;
        }
        var f = elementDetail.ParsedText[1];
        if (f.Length < 1 || f.Length > 7)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementT printer ID len wrong");
            return validationResult;
        }
        foreach (var c in f.Span)
            if (!char.IsLetterOrDigit(c))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementT printer ID invalid char");
                return validationResult;
            }
        return validationResult;
    }
}

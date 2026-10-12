using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementYValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 6 || elementDetail.Length > 55)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementY length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2 || elementDetail.ParsedText[1].Length < 1 || elementDetail.ParsedText[1].Length > 25)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementY FF ID wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 2 && elementDetail.ParsedText[2].Length > 25)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementY FF ID too long");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 3 && elementDetail.ParsedText[3].Length > 25)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementY tier too long");
            return validationResult;
        }
        return validationResult;
    }
}

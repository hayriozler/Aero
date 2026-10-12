using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementRValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 6 || elementDetail.Length > 64)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementR length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length != 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementR structure wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText[1].Length < 1 || elementDetail.ParsedText[1].Length > 59)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementR free text len wrong");
        }
        return validationResult;
    }
}

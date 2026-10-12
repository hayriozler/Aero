using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementNValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length != 18)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementN length must be 18");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length != 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementN structure wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText[1].Length != 13)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementN tag field must be 13");
            return validationResult;
        }
        foreach (var c in elementDetail.ParsedText[1].Span)
            if (!char.IsDigit(c))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementN tag must be digits");
                return validationResult;
            }
        return validationResult;
    }
}

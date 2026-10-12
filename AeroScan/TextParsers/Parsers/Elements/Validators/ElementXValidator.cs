using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementXValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 6 || elementDetail.Length > 63)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2 || elementDetail.ParsedText[1].IsEmpty)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX instruction missing");
            return validationResult;
        }
        if (elementDetail.ParsedText[1].Length != 3)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX instruction must be 3");
            return validationResult;
        }
        if (!Consts.SCREEN_INSTRUCTION_ARRAY.ContainsSpan(elementDetail.ParsedText[1].Span))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX instruction invalid");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 2)
        {
            if (elementDetail.ParsedText[2].Length != 3)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX result must be 3");
                return validationResult;
            }
            if (!Consts.SCREEN_RESULT_ARRAY.ContainsSpan(elementDetail.ParsedText[2].Span))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX result invalid");
                return validationResult;
            }
        }
        if (elementDetail.ParsedText.Length > 3)
        {
            if (elementDetail.ParsedText[3].Length != 1)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX reason must be 1");
                return validationResult;
            }
            if (!Consts.SCREEN_RESULT_REASON_ARRAY.ContainsSpan(elementDetail.ParsedText[3].Span))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX reason invalid");
                return validationResult;
            }
        }
        if (elementDetail.ParsedText.Length > 4 && (elementDetail.ParsedText[4].Length < 2 || elementDetail.ParsedText[4].Length > 4))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX method len wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 5 && elementDetail.ParsedText[5].Length > 8)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX autograph too long");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 6 && elementDetail.ParsedText[6].Length > 38)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementX free text too long");
            return validationResult;
        }
        return validationResult;
    }
}

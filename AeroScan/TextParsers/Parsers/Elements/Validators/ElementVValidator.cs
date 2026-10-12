using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementVValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 10 || elementDetail.Length > 41)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV fields missing");
            return validationResult;
        }
        var f1 = elementDetail.ParsedText[1];
        if (f1.Length != 5)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV combined field must be 5");
            return validationResult;
        }
        if (!char.IsDigit(f1.Span[0]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV version must be digit");
            return validationResult;
        }
        if (!Consts.BAG_SOURCE_INDICATOR_ARRAY.ContainsChar(f1.Span[1]))
        {
            validationResult.AddError(ErrorCodes.BAGGAGE_SOURCE_INDICATOR_NOT_CORRECT, "ElementV indicator invalid");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirportCode(f1.Slice(2, 3)))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV airport wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 2 && elementDetail.ParsedText[2].Length > 5)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV partNo too long");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 3 && elementDetail.ParsedText[3].Length > 10)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV msgRef too long");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 4 && (elementDetail.ParsedText[4].Length != 1 || elementDetail.ParsedText[4].Span[0] != 'A'))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV ackReq must be A");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 5 && elementDetail.ParsedText[5].Length > 10)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementV encrypt too long");
            return validationResult;
        }
        return validationResult;
    }
}

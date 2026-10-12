using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementOValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 22 || elementDetail.Length > 25)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 5)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO fields missing");
            return validationResult;
        }
        var ff = elementDetail.ParsedText[1];
        if (ff.Length < 5 || ff.Length > 8)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO flight len wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirlineFlightNumber(ff, out _))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO flight wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateIataDate(elementDetail.ParsedText[2]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO date wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirportCode(elementDetail.ParsedText[3]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO dest wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText[4].Length != 1 || !char.IsLetter(elementDetail.ParsedText[4].Span[0]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementO class wrong");
            return validationResult;
        }
        return validationResult;
    }
}

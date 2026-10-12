using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementSValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 6 || elementDetail.Length > 26)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementS length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementS atl missing");
            return validationResult;
        }
        switch (elementDetail.ParsedText.Length)
        {
            case 2:
                if (Consts.PASSENGER_STATUS_ARRAY.ContainsSpan(elementDetail.ParsedText[1].Span))
                {
                    validationResult.AddError(ErrorCodes.PASSENGER_STATUS_VALUE_NOT_CORRECT, "Passenger status value is not correct");
                }
                break;
            case 5:
                if (Consts.YesNoValues.ContainsSpan(elementDetail.ParsedText[4].Span))
                {
                    validationResult.AddError(ErrorCodes.PASSENGER_PROFIL_STATUS_VALUE_NOT_CORRECT, "Passenger profile is not correct");
                }
                break;
            case 6:
                if (Consts.BagTagStatusValues.ContainsSpan(elementDetail.ParsedText[5].Span))
                {
                    validationResult.AddError(ErrorCodes.BAGTAG_STATUS_VALUE_NOT_CORRECT, "Bagtag status is not correct");
                }
                break;
            
        } 
        return validationResult;
    }
    
}

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementFValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 15 || elementDetail.Length > 25)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementF length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 4)
        {
            validationResult.AddError(ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT, "ElementF fields missing");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirlineFlightNumber(elementDetail.ParsedText[1], out _))
        {
            validationResult.AddError(ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT, "ElementF flight wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateIataDate(elementDetail.ParsedText[2]))
        {
            validationResult.AddError(ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT, "ElementF date wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirportCode(elementDetail.ParsedText[3]))
        {
            validationResult.AddError(ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT, "ElementF dest wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 4 && !elementDetail.ParsedText[4].IsEmpty)
            if (elementDetail.ParsedText[4].Length != 1 || !char.IsLetter(elementDetail.ParsedText[4].Span[0]))
            {
                validationResult.AddError(ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT, "ElementF class wrong");
                return validationResult;
            }
        return validationResult;
    }
}

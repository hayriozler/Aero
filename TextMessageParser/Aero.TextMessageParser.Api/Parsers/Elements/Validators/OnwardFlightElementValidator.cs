using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class OnwardFlightElementValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 22 || elementDetail.Length > 25)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "OnwardFlightElement length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 5)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, "OnwardFlightElement fields missing");
            return validationResult;
        }
        var ff = elementDetail.ParsedText[1];
        if (ff.Length < 5 || ff.Length > 8)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "OnwardFlightElement flight len wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirlineFlightNumber(ff, out _))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "OnwardFlightElement flight wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateIataDate(elementDetail.ParsedText[2]))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "OnwardFlightElement date wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirportCode(elementDetail.ParsedText[3]))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "OnwardFlightElement dest wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText[4].Length != 1 || !char.IsLetter(elementDetail.ParsedText[4].Span[0]))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "OnwardFlightElement class wrong");
            return validationResult;
        }
        return validationResult;
    }
}

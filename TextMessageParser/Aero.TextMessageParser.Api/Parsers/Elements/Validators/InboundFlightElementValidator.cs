using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class InboundFlightElementValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 20 || elementDetail.Length > 25)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "InboundFlightElement length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 4)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, "InboundFlightElement fields missing");
            return validationResult;
        }
        var f1 = elementDetail.ParsedText[1];
        if (!ValidationHelper.ValidateAirlineFlightNumber(f1, out _))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "InboundFlightElement flight wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateIataDate(elementDetail.ParsedText[2]))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "InboundFlightElement date wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirportCode(elementDetail.ParsedText[3]))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "InboundFlightElement origin wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 4 && !elementDetail.ParsedText[4].IsEmpty)
            if (elementDetail.ParsedText[4].Length != 1 || !char.IsLetter(elementDetail.ParsedText[4].Span[0]))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "InboundFlightElement class wrong");
                return validationResult;
            }
        return validationResult;
    }
}

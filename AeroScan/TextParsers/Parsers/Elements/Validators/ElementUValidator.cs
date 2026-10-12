using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementUValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 8 || elementDetail.Length > 53)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU length wrong");
            return validationResult;
        }
        for (int i = 1; i < elementDetail.ParsedText.Length; i++)
        {
            var f = elementDetail.ParsedText[i];
            switch (i)
            {
                case 1:
                    if (f.Length > 10)
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU stowage ID too long");
                        return validationResult;
                    }
                    break;
                case 2:
                    if (f.Length > 5)
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU compartment too long");
                        return validationResult;
                    }
                    break;
                case 3:
                    if (!Consts.TYPE_OF_BAGGAGE_ARRAY.ContainsSpan(f.Span))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU bag type invalid");
                        return validationResult;
                    }
                    break;
                case 4:
                    if (f.Length != 1 || !char.IsLetter(f.Span[0]))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU class wrong");
                        return validationResult;
                    }
                    break;
                case 5:
                    if (!ValidationHelper.ValidateAirportCode(f))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU dest wrong");
                        return validationResult;
                    }
                    break;
                case 6:
                    if (!Consts.SealedIndicatorValues.ContainsSpan(f.Span))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU sealed wrong");
                        return validationResult;
                    }
                    break;
                case 7:
                    if (!ValidationHelper.ValidateAirlineFlightNumber(f, out _))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU conn flight wrong");
                        return validationResult;
                    }
                    break;
                case 8:
                    if (!ValidationHelper.ValidateIataDate(f))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU conn date wrong");
                        return validationResult;
                    }
                    break;
                case 9:
                    if (!ValidationHelper.ValidateAirportCode(f))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementU transfer wrong");
                        return validationResult;
                    }
                    break;
            }
        }
        return validationResult;
    }
}

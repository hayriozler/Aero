using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementIValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 22 || elementDetail.Length > 25)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 4)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI fields missing");
            return validationResult;
        }
        var f1 = elementDetail.ParsedText[1];
        if (f1.Length < 5 || f1.Length > 8)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI flight len wrong");
            return validationResult;
        }
        int airlineLen = char.IsDigit(f1.Span[2]) ? 2 : 3;
        for (int i = 0; i < airlineLen; i++)
            if (!char.IsLetter(f1.Span[i]))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI airline wrong");
                return validationResult;
            }
        var flightNo = f1.Slice(airlineLen);
        int digits = 0;
        foreach (var c in flightNo.Span)
        {
            if (!char.IsDigit(c))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI flight digits wrong");
                return validationResult;
            }
            digits++;
        }
        if (digits < 1 || digits > 4)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI flight digit count wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateIataDate(elementDetail.ParsedText[2]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI date wrong");
            return validationResult;
        }
        if (!ValidationHelper.ValidateAirportCode(elementDetail.ParsedText[3]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI origin wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 4 && !elementDetail.ParsedText[4].IsEmpty)
            if (elementDetail.ParsedText[4].Length != 1 || !char.IsLetter(elementDetail.ParsedText[4].Span[0]))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementI class wrong");
                return validationResult;
            }
        return validationResult;
    }
}

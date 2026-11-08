using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class PassengerNameElementValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 7 || elementDetail.Length > 64)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "PassengerNameElement length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, "PassengerNameElement surname missing");
            return validationResult;
        }
        var f1 = elementDetail.ParsedText[1];
        int digits = 0;
        while (digits < f1.Length && char.IsDigit(f1.Span[digits])) digits++;
        if (digits > 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "PassengerNameElement pax count > 2 digits");
            return validationResult;
        }
        var surname = f1.Slice(digits);
        if (surname.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "PassengerNameElement surname too short");
            return validationResult;
        }
        foreach (var c in surname.Span)
            if (!char.IsLetter(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "PassengerNameElement surname must be letters");
                return validationResult;
            }
        for (int i = 2; i < elementDetail.ParsedText.Length; i++)
            if (elementDetail.ParsedText[i].IsEmpty)
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, $"PassengerNameElement name field {i} empty");
                return validationResult;
            }
        return validationResult;
    }
}

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementPValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 7 || elementDetail.Length > 64)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementP length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementP surname missing");
            return validationResult;
        }
        var f1 = elementDetail.ParsedText[1];
        int digits = 0;
        while (digits < f1.Length && char.IsDigit(f1.Span[digits])) digits++;
        if (digits > 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementP pax count > 2 digits");
            return validationResult;
        }
        var surname = f1.Slice(digits);
        if (surname.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementP surname too short");
            return validationResult;
        }
        foreach (var c in surname.Span)
            if (!char.IsLetter(c))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementP surname must be letters");
                return validationResult;
            }
        for (int i = 2; i < elementDetail.ParsedText.Length; i++)
            if (elementDetail.ParsedText[i].IsEmpty)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, $"ElementP name field {i} empty");
                return validationResult;
            }
        return validationResult;
    }
}

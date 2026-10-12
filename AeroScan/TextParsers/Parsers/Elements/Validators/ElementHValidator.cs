using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementHValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 11 || elementDetail.Length > 28)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementH length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementH field missing");
            return validationResult;
        }
        for (int i = 1; i < elementDetail.ParsedText.Length; i++)
        {
            int maxLen = i switch { 1 => 6, 2 => 6, _ => 8 };
            var f = elementDetail.ParsedText[i];
            if (f.Length > maxLen)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, $"ElementH field {i} too long");
                return validationResult;
            }
            foreach (var c in f.Span)
                if (!char.IsLetterOrDigit(c))
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, $"ElementH field {i} invalid char");
                    return validationResult;
                }
        }
        return validationResult;
    }
}

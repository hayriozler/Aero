using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementNValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length != 18)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementN length must be 18");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length != 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementN structure wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText[1].Length != 13)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementN tag field must be 13");
            return validationResult;
        }
        foreach (var c in elementDetail.ParsedText[1].Span)
            if (!char.IsDigit(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "ElementN tag must be digits");
                return validationResult;
            }
        if (elementDetail.ParsedText[1].Span[10..].SequenceEqual("000"))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "ElementN number of consecutive tags must be at least 001");
            return validationResult;
        }
        return validationResult;
    }
}

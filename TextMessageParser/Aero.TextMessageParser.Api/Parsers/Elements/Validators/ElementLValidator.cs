using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementLValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length != 10 && elementDetail.Length != 11)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementL length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length != 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementL structure wrong");
            return validationResult;
        }
        var f = elementDetail.ParsedText[1];
        if (f.Length != 5 && f.Length != 6)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementL PNR length wrong");
            return validationResult;
        }
        foreach (var c in f.Span)
            if (!char.IsLetterOrDigit(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "ElementL PNR invalid char");
                return validationResult;
            }
        return validationResult;
    }
}

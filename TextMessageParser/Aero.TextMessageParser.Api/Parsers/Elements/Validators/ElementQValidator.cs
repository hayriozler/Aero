using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementQValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length != 8)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementQ length must be 8");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length != 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementQ structure wrong");
            return validationResult;
        }
        var f = elementDetail.ParsedText[1];
        if (f.Length != 3)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementQ load sequence number must be 3");
            return validationResult;
        }
        foreach (var c in f.Span)
            if (!char.IsDigit(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "ElementQ load sequence number must be digits");
                return validationResult;
            }
        return validationResult;
    }
}

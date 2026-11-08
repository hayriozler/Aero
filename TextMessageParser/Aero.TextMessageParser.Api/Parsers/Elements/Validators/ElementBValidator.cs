using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementBValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);

        if (elementDetail.Length < 18 || elementDetail.Length > 23)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementB length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 3)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, "ElementB fields missing");
            return validationResult;
        }
        if (!Consts.BAG_STATUS_CODE_ARRAY.ContainsSpan(elementDetail.ParsedText[1].Span))
        {
            validationResult.AddError(ErrorCodes.BAG_STATUS_VALUE_NOT_CORRECT, "ElementB status code wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText[2].Length != 13)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementB tag length wrong");
            return validationResult;
        }
        foreach (var c in elementDetail.ParsedText[2].Span)
        {
            if (!char.IsDigit(c))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "ElementB tag must be digits");
                return validationResult;
            }
        }
        return validationResult;
    }
}

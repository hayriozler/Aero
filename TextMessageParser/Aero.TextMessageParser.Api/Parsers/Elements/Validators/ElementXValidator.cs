using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class ElementXValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 6 || elementDetail.Length > 63)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX length wrong");
            return validationResult;
        }
        var fields = elementDetail.ParsedText;

        if (Has(fields, 1))
        {
            if (fields[1].Length != 3)
            {
                validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX instruction must be 3");
                return validationResult;
            }
            if (!Consts.SCREEN_INSTRUCTION_ARRAY.ContainsSpan(fields[1].Span))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "ElementX instruction invalid");
                return validationResult;
            }
        }
        if (Has(fields, 2))
        {
            if (fields[2].Length != 3)
            {
                validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX result must be 3");
                return validationResult;
            }
            if (!Consts.SCREEN_RESULT_ARRAY.ContainsSpan(fields[2].Span))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "ElementX result invalid");
                return validationResult;
            }
        }
        if (Has(fields, 3))
        {
            if (fields[3].Length != 1)
            {
                validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX reason must be 1");
                return validationResult;
            }
            if (!Consts.SCREEN_RESULT_REASON_ARRAY.ContainsSpan(fields[3].Span))
            {
                validationResult.AddError(ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "ElementX reason invalid");
                return validationResult;
            }
        }
        if (Has(fields, 4) && (fields[4].Length < 2 || fields[4].Length > 4))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX method len wrong");
            return validationResult;
        }
        if (Has(fields, 5) && fields[5].Length > 8)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX autograph too long");
            return validationResult;
        }
        if (Has(fields, 6) && fields[6].Length > 38)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "ElementX free text too long");
            return validationResult;
        }
        return validationResult;
    }

    private static bool Has(ReadOnlyMemory<char>[] fields, int index) => fields.Length > index && !fields[index].IsEmpty;
}

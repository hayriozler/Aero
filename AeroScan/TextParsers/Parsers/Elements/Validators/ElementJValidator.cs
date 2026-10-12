using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementJValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 9 || elementDetail.Length > 55)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ secondary code missing");
            return validationResult;
        }
        if (!Consts.SECONDARY_CODE_ARRAY.ContainsSpan(elementDetail.ParsedText[1].Span))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ secondary code invalid");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 2)
        {
            var a = elementDetail.ParsedText[2];
            if (a.Length < 2 || a.Length > 9)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ agent len wrong");
                return validationResult;
            }
        }
        if (elementDetail.ParsedText.Length > 3)
        {
            var s = elementDetail.ParsedText[3];
            if (s.Length < 2 || s.Length > 8)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ scanner len wrong");
                return validationResult;
            }
        }
        if (elementDetail.ParsedText.Length > 4 && !ValidationHelper.ValidateIataDate(elementDetail.ParsedText[4]))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ date wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 5)
        {
            var t = elementDetail.ParsedText[5];
            if (t.Length != 5 && t.Length != 7)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ time len wrong");
                return validationResult;
            }
            if (!Consts.ValidTimeSuffixes.ContainsSpan(t.Span.Slice(t.Length - 1, 1)))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ time suffix wrong");
                return validationResult;
            }
            for (int i = 0; i < t.Length - 1; i++)
                if (!char.IsDigit(t.Span[i]))
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ time digits wrong");
                    return validationResult;
                }
        }
        if (elementDetail.ParsedText.Length > 6)
        {
            var rl = elementDetail.ParsedText[6];
            if (rl.Length < 2 || rl.Length > 8 || !char.IsLetter(rl.Span[0]))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ readLoc wrong");
                return validationResult;
            }
        }
        if (elementDetail.ParsedText.Length > 7)
        {
            var sl = elementDetail.ParsedText[7];
            if (sl.Length < 2 || sl.Length > 8 || !char.IsLetter(sl.Span[0]))
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementJ sendLoc wrong");
                return validationResult;
            }
        }
        return validationResult;
    }
}

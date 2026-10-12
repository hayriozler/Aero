using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementWValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult();
        if (elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 8 || elementDetail.Length > 53)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW PWI missing");
            return validationResult;
        }
        if (!Consts.PIECE_WEIGHT_ARRAY.ContainsSpan(elementDetail.ParsedText[1].Span))
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW PWI must be K/L/P");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 2)
        {
            var f = elementDetail.ParsedText[2];
            if (f.Length > 3)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW pieces count too long");
                return validationResult;
            }
            foreach (var c in f.Span)
                if (!char.IsDigit(c))
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW pieces must be numeric");
                    return validationResult;
                }
        }
        if (elementDetail.ParsedText.Length > 3)
        {
            var f = elementDetail.ParsedText[3];
            if (f.Length > 5)
            {
                validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW weight too long");
                return validationResult;
            }
            foreach (var c in f.Span)
                if (!char.IsDigit(c))
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW weight must be numeric");
                    return validationResult;
                }
        }
        if (elementDetail.ParsedText.Length > 4 && Consts.UnitValues.ContainsSpan(elementDetail.ParsedText[4].Span))
        {
            if (elementDetail.ParsedText.Length > 5)
            {
                var f = elementDetail.ParsedText[5];
                if (f.Length > 3)
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW length dim wrong");
                    return validationResult;
                }
                foreach (var c in f.Span)
                    if (!char.IsDigit(c))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW dim must be numeric");
                        return validationResult;
                    }
            }
            if (elementDetail.ParsedText.Length > 6)
            {
                var f = elementDetail.ParsedText[6];
                if (f.Length > 3)
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW width dim wrong");
                    return validationResult;
                }
                foreach (var c in f.Span)
                    if (!char.IsDigit(c))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW dim must be numeric");
                        return validationResult;
                    }
            }
            if (elementDetail.ParsedText.Length > 7)
            {
                var f = elementDetail.ParsedText[7];
                if (f.Length > 3)
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW height dim wrong");
                    return validationResult;
                }
                foreach (var c in f.Span)
                    if (!char.IsDigit(c))
                    {
                        validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW dim must be numeric");
                        return validationResult;
                    }
            }
        }
        if (elementDetail.ParsedText.Length > 8 && elementDetail.ParsedText[8].Length > 4)
        {
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementW bagtype too long");
        }
        return validationResult;
    }
}

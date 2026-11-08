using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class BaggageWeightElementValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 8 || elementDetail.Length > 53)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "BaggageWeightElement length wrong");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length < 2)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_FIELD_MISSING, "BaggageWeightElement PWI missing");
            return validationResult;
        }
        if (!Consts.PIECE_WEIGHT_ARRAY.ContainsSpan(elementDetail.ParsedText[1].Span))
        {
            validationResult.AddError(ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "BaggageWeightElement PWI must be K/L/P");
            return validationResult;
        }
        if (elementDetail.ParsedText.Length > 2)
        {
            var f = elementDetail.ParsedText[2];
            if (f.Length > 3)
            {
                validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "BaggageWeightElement pieces count too long");
                return validationResult;
            }
            foreach (var c in f.Span)
                if (!char.IsDigit(c))
                {
                    validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement pieces must be numeric");
                    return validationResult;
                }
        }
        if (elementDetail.ParsedText.Length > 3)
        {
            var f = elementDetail.ParsedText[3];
            if (f.Length > 5)
            {
                validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "BaggageWeightElement weight too long");
                return validationResult;
            }
            foreach (var c in f.Span)
                if (!char.IsDigit(c))
                {
                    validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement weight must be numeric");
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
                    validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "BaggageWeightElement length dim wrong");
                    return validationResult;
                }
                foreach (var c in f.Span)
                    if (!char.IsDigit(c))
                    {
                        validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement dim must be numeric");
                        return validationResult;
                    }
            }
            if (elementDetail.ParsedText.Length > 6)
            {
                var f = elementDetail.ParsedText[6];
                if (f.Length > 3)
                {
                    validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement width dim wrong");
                    return validationResult;
                }
                foreach (var c in f.Span)
                    if (!char.IsDigit(c))
                    {
                        validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement dim must be numeric");
                        return validationResult;
                    }
            }
            if (elementDetail.ParsedText.Length > 7)
            {
                var f = elementDetail.ParsedText[7];
                if (f.Length > 3)
                {
                    validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement height dim wrong");
                    return validationResult;
                }
                foreach (var c in f.Span)
                    if (!char.IsDigit(c))
                    {
                        validationResult.AddError(ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "BaggageWeightElement dim must be numeric");
                        return validationResult;
                    }
            }
        }
        if (elementDetail.ParsedText.Length > 8 && elementDetail.ParsedText[8].Length > 4)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "BaggageWeightElement bagtype too long");
        }
        return validationResult;
    }
}

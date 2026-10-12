using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

public class ElementAValidator : IElementValidator
{
    public ElementValidationResult Validate(ElementDetail elementDetail)
    {        
        var validationResult = new ElementValidationResult();
        if(elementDetail.Requirement != ElementRequirement.Mandatory) return validationResult;
        if (elementDetail.Length < 14 || elementDetail.Length > 63) 
        { 
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementA length is not correct"); 
            return validationResult; 
        }
        if (elementDetail.ParsedText.Length < 1)             
        { 
            validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementA required fields missing"); 
            return validationResult; 
        }
        // First Item always be Identifier
        switch (elementDetail.ParsedText.Length)
        {
            case 2:
                if (elementDetail.ParsedText[1].IsEmpty || elementDetail.ParsedText[1].Length < 1 || elementDetail.ParsedText[1].Length > 10)
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementA ref number length wrong");
                  
                }
                break;
            case 3:
                if (elementDetail.ParsedText[2].IsEmpty || elementDetail.ParsedText[2].Length < 3 || elementDetail.ParsedText[2].Length > 7)
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementA status length wrong");
                }
                break;

            case 4:
                if (elementDetail.ParsedText[3].IsEmpty || !Consts.ACK_NAK_ARRAY.ContainsSpan(elementDetail.ParsedText[3].Span))
                {
                    validationResult.AddError(ErrorCodes.ACK_NAK_VALUE_NOT_CORRECT, "ElementA ack/nak value wrong");
                }
                break;

            case 5:
                if (elementDetail.ParsedText[4].IsEmpty || elementDetail.ParsedText[4].Length < 4 || elementDetail.ParsedText[4].Length > 35)
                {
                    validationResult.AddError(ErrorCodes.ERROR_CODE_MSG_LEN_NOT_CORRECT, "ElementA free text too long");
                }
                break;
        }
        return validationResult;
    }
}
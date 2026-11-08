using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementX(IElementValidator validator) : Element(Consts.X)
{
    public string SecurityScreenInstruction { get; private set; } = string.Empty;
    public string SecurityScreenResult { get; private set; } = string.Empty;
    public string SecurityScreenResultReason { get; private set; } = string.Empty;
    public string SecurityScreenMethod { get; private set; } = string.Empty;
    public string Autograph { get; private set; } = string.Empty;
    public string FreeText { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementX(validator);
        var parsedText = elementDetail.ParsedText;
        result.SecurityScreenInstruction  = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.SecurityScreenResult       = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        result.SecurityScreenResultReason = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        result.SecurityScreenMethod       = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        result.Autograph                  = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        result.FreeText                   = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
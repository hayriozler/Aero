
using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementX(IElementValidator validator) : Element(Consts.X)
{
    public string SecurityScreenInstruction  { get; private set; } = string.Empty;
    public string SecurityScreenResult       { get; private set; } = string.Empty;
    public string SecurityScreenResultReason { get; private set; } = string.Empty;
    public string SecurityScreenMethod       { get; private set; } = string.Empty;
    public string Autograph                  { get; private set; } = string.Empty;
    public string FreeText                   { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        SecurityScreenInstruction  = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        SecurityScreenResult       = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        SecurityScreenResultReason = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        SecurityScreenMethod       = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        Autograph                  = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        FreeText                   = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        return new(this, validationResult);
    }
}


using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementJ(IElementValidator validator) : Element(Consts.J)
{
    public string SecondaryCode           { get; private set; } = string.Empty;
    public string AgentIdentification     { get; private set; } = string.Empty;
    public string ScannerIdentification   { get; private set; } = string.Empty;
    public string ProcessDate             { get; private set; } = string.Empty;
    public string ProcessTime             { get; private set; } = string.Empty;
    public string ReadLocation            { get; private set; } = string.Empty;
    public string SendLocation { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        SecondaryCode         = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        AgentIdentification   = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty; 
        ScannerIdentification = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        ProcessDate           = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        ProcessTime           = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        ReadLocation          = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        SendLocation          = parsedText.Length > 7 ? parsedText[7].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
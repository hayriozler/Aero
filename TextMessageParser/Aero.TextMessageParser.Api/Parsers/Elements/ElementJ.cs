using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementJ(IElementValidator validator) : Element(Consts.J)
{
    public string SecondaryCode { get; private set; } = string.Empty;
    public string AgentIdentification { get; private set; } = string.Empty;
    public string ScannerIdentification { get; private set; } = string.Empty;
    public string ProcessDate { get; private set; } = string.Empty;
    public string ProcessTime { get; private set; } = string.Empty;
    public string ReadLocation { get; private set; } = string.Empty;
    public string SendLocation { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementJ(validator);
        var parsedText = elementDetail.ParsedText;
        result.SecondaryCode         = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.AgentIdentification   = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        result.ScannerIdentification = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        result.ProcessDate           = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        result.ProcessTime           = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        result.ReadLocation          = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        result.SendLocation          = parsedText.Length > 7 ? parsedText[7].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
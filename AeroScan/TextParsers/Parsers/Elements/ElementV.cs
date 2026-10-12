
using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementV(IElementValidator validator) : Element(Consts.V)
{
    public char DataDictionaryVersion  { get; private set; } 
    public char BaggageSourceIndicator { get; private set; }
    public string AirportCode          { get; private set; } = string.Empty;
    public string PartNumber           { get; private set; } = string.Empty;
    public string MessageRefNumber     { get; private set; } = string.Empty;
    public char? AckRequest            { get; private set; }
    public string EncryptionKey { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var v = validator.Validate(elementDetail);
        if (!v.IsValid) return new(this, v);
        var parsedText = elementDetail.ParsedText;
        var f1 = parsedText.Length > 1 ? parsedText[1] : default;
        DataDictionaryVersion  = f1[..1].Span[0];
        BaggageSourceIndicator = f1.Slice(1, 1).Span[0];
        AirportCode            = f1.Slice(2, 3).ToString();
        PartNumber = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        MessageRefNumber = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        AckRequest = parsedText.Length > 4 ? parsedText[4].ToString()[0] : null;
        EncryptionKey   = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        return new(this, v);
    }
}
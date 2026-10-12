

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementS(IElementValidator validator) : Element(Consts.S)
{
    public char AuthorityToLoad { get; private set; }
    public string? SeatNumber   { get; private set; }
    public char PassengerStatus { get; private set; } 
    public string SequenceNumber { get; private set; } = string.Empty;
    public string SecurityNumber { get; private set; } = string.Empty;
    public char PassengerProfileStatus { get; private set; }
    public char AuthorityToTransport   { get; private set; }
    public char BaggageTagStatus       { get; private set; }

    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        AuthorityToLoad = parsedText.Length > 1 ? parsedText[1].Span[0] : default;
        SeatNumber = parsedText.Length > 2 ? parsedText[2].ToString() : default;
        PassengerStatus = parsedText.Length > 3 ? parsedText[3].Span[0] : default;
        SequenceNumber = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        SecurityNumber = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        PassengerProfileStatus = parsedText.Length > 6 ? parsedText[6].Span[0] : default;
        AuthorityToTransport= parsedText.Length > 7 ? parsedText[7].Span[0] : default;
        BaggageTagStatus= parsedText.Length > 8 ? parsedText[8].Span[0] : default;
        return new(this, validationResult);
    }
}

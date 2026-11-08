using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementS(IElementValidator validator) : Element(Consts.S)
{
    public char AuthorityToLoad { get; private set; }
    public string? SeatNumber { get; private set; }
    public char PassengerStatus { get; private set; }
    public string SequenceNumber { get; private set; } = string.Empty;
    public string SecurityNumber { get; private set; } = string.Empty;
    public char PassengerProfileStatus { get; private set; }
    public char AuthorityToTransport { get; private set; }
    public char BaggageTagStatus { get; private set; }

    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementS(validator);
        var parsedText = elementDetail.ParsedText;
        result.AuthorityToLoad = parsedText.Length > 1 ? parsedText[1].Span[0] : default;
        result.SeatNumber = parsedText.Length > 2 ? parsedText[2].ToString() : default;
        result.PassengerStatus = parsedText.Length > 3 ? parsedText[3].Span[0] : default;
        result.SequenceNumber = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        result.SecurityNumber = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        result.PassengerProfileStatus = parsedText.Length > 6 ? parsedText[6].Span[0] : default;
        result.AuthorityToTransport= parsedText.Length > 7 ? parsedText[7].Span[0] : default;
        result.BaggageTagStatus= parsedText.Length > 8 ? parsedText[8].Span[0] : default;
        return new(result, validationResult);
    }
}

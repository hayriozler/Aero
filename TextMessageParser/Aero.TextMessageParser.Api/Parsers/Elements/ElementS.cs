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
        result.AuthorityToLoad = parsedText.Length > 1 ? FirstCharOrDefault(parsedText[1]) : default;
        result.SeatNumber = parsedText.Length > 2 ? TextOrNull(parsedText[2]) : default;
        result.PassengerStatus = parsedText.Length > 3 ? FirstCharOrDefault(parsedText[3]) : default;
        result.SequenceNumber = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        result.SecurityNumber = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        result.PassengerProfileStatus = parsedText.Length > 6 ? FirstCharOrDefault(parsedText[6]) : default;
        result.AuthorityToTransport = parsedText.Length > 7 ? FirstCharOrDefault(parsedText[7]) : default;
        result.BaggageTagStatus = parsedText.Length > 8 ? FirstCharOrDefault(parsedText[8]) : default;
        return new(result, validationResult);
    }
}

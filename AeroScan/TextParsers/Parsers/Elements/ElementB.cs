using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

//Baggage Irregularities
public class ElementB(IElementValidator validator) : Element(Consts.B)
{
    public string BaggageStatusCode        { get; private set; } = string.Empty;
    public string BaggageTagNumber         { get; private set; } = string.Empty;
    public string BaggageTagConsecutiveTag { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);

        var parsedText = elementDetail.ParsedText;
        BaggageStatusCode = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        BaggageTagNumber = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        BaggageTagConsecutiveTag = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
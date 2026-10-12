

using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementD(IElementValidator validator) : Element(Consts.D)
{
    public string BagCheckInLocationIdentifier { get; private set; } = string.Empty;
    public string BagCheckInLocationDescription  { get; private set; } = string.Empty;
    public string BagCheckInDate                 { get; private set; } = string.Empty;
    public string BagCheckInTime                 { get; private set; } = string.Empty;
    public string CarriageMedium                 { get; private set; } = string.Empty;
    public string TransportId                    { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;

        BagCheckInLocationIdentifier  = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        BagCheckInLocationDescription = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        BagCheckInDate  = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        BagCheckInTime  = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        CarriageMedium  = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        TransportId     = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        return new(this, validationResult);
    }
}
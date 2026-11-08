using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementD(IElementValidator validator) : Element(Consts.D)
{
    public string BagCheckInLocationIdentifier { get; private set; } = string.Empty;
    public string BagCheckInLocationDescription { get; private set; } = string.Empty;
    public string BagCheckInDate { get; private set; } = string.Empty;
    public string BagCheckInTime { get; private set; } = string.Empty;
    public string CarriageMedium { get; private set; } = string.Empty;
    public string TransportId { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementD(validator);
        var parsedText = elementDetail.ParsedText;

        result.BagCheckInLocationIdentifier  = parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.BagCheckInLocationDescription = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        result.BagCheckInDate  = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        result.BagCheckInTime  = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        result.CarriageMedium  = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
        result.TransportId     = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
        return new(result, validationResult);
    }
}
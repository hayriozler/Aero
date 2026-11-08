using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementW(IElementValidator validator) : Element(Consts.W)
{
    public string PiecesWeightIndicator { get; private set; } = string.Empty;
    public string NumberOfCheckedBag { get; private set; } = string.Empty;
    public string CheckedWeight { get; private set; } = string.Empty;
    public string UncheckedWeight { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public string LengthOfBag { get; private set; } = string.Empty;
    public string WidthOfBag { get; private set; } = string.Empty;
    public string HeightOfBag { get; private set; } = string.Empty;
    public string BaggageTypeCode { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementW(validator);
        var parsedText = elementDetail.ParsedText;
        result.PiecesWeightIndicator =  parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        result.NumberOfCheckedBag    = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        result.CheckedWeight         = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        if (parsedText.Length > 4)
        {
            bool isUnit = parsedText[4].Span.SequenceEqual("CM".AsSpan()) || parsedText[4].Span.SequenceEqual("IN".AsSpan());
            if (isUnit)
            {
                result.Unit = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
                result.LengthOfBag  = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
                result.WidthOfBag   = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
                result.HeightOfBag  = parsedText.Length > 7 ? parsedText[7].ToString() : string.Empty;
                result.BaggageTypeCode = parsedText.Length > 8 ? parsedText[8].ToString() : string.Empty;
            }
            else result.UncheckedWeight = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        }
        return new(result, validationResult);
    }
}
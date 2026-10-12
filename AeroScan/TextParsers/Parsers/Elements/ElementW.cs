
using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public class ElementW(IElementValidator validator) : Element(Consts.W)
{
    public string PiecesWeightIndicator { get; private set; } = string.Empty;
    public string NumberOfCheckedBag    { get; private set; } = string.Empty;
    public string CheckedWeight         { get; private set; } = string.Empty;
    public string UncheckedWeight       { get; private set; } = string.Empty;
    public string Unit                  { get; private set; } = string.Empty;
    public string LengthOfBag           { get; private set; } = string.Empty;
    public string WidthOfBag            { get; private set; } = string.Empty;
    public string HeightOfBag           { get; private set; } = string.Empty;
    public string BaggageTypeCode       { get; private set; } = string.Empty;
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var parsedText = elementDetail.ParsedText;
        PiecesWeightIndicator =  parsedText.Length > 1 ? parsedText[1].ToString() : string.Empty;
        NumberOfCheckedBag    = parsedText.Length > 2 ? parsedText[2].ToString() : string.Empty;
        CheckedWeight         = parsedText.Length > 3 ? parsedText[3].ToString() : string.Empty;
        if (parsedText.Length > 4)
        {
            bool isUnit = parsedText[4].Span.SequenceEqual("CM".AsSpan()) || parsedText[4].Span.SequenceEqual("IN".AsSpan());
            if (isUnit)
            {
                Unit = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
                LengthOfBag  = parsedText.Length > 5 ? parsedText[5].ToString() : string.Empty;
                WidthOfBag   = parsedText.Length > 6 ? parsedText[6].ToString() : string.Empty;
                HeightOfBag  = parsedText.Length > 7 ? parsedText[7].ToString() : string.Empty;
                BaggageTypeCode = parsedText.Length > 8 ? parsedText[8].ToString() : string.Empty;
            }
            else UncheckedWeight = parsedText.Length > 4 ? parsedText[4].ToString() : string.Empty;
        }
        return new(this, validationResult);
    }
}
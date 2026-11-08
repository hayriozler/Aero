using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.Parsers.Elements;

public class ElementP(IElementValidator validator) : Element(Consts.P)
{
    public string NumberOfPassengersWithSurname { get; private set; } = string.Empty;
    public string PassengerSurname { get; private set; } = string.Empty;
    public string PassengerGivenName { get; private set; } = string.Empty;
    public List<string> AdditionalGivenNames { get; private set; } = [];
    public override ElementResult Parse(ElementDetail elementDetail)
    {
        var validationResult = validator.Validate(elementDetail);
        if (!validationResult.IsValid) return new(this, validationResult);
        var result = new ElementP(validator);
        if (elementDetail.ParsedText.Length < 2) return new(this, validationResult);
        int digits = 0;
        while (digits < elementDetail.ParsedText[1].Length && char.IsDigit(elementDetail.ParsedText[1].Span[digits]))
        {
            digits++;
        }
        if (digits > 0)
        {
            result.NumberOfPassengersWithSurname = elementDetail.ParsedText[1][..digits].ToString();
            result.PassengerSurname = elementDetail.ParsedText[1][digits..].ToString();
        }
        else
        {
            result.PassengerSurname = elementDetail.ParsedText[1].ToString();
        }
        if (elementDetail.ParsedText.Length > 2)
            result.PassengerGivenName = elementDetail.ParsedText[2].ToString();

        for (int i = 3; i < elementDetail.ParsedText.Length; i++)
            result.AdditionalGivenNames.Add(elementDetail.ParsedText[i].ToString());
        return new(result, validationResult);
    }
}
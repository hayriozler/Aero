namespace Aero.TextMessageParser.Api.Dtos;

public sealed record BaggageWeightDto(
    string PiecesWeightIndicator,
    string NumberOfCheckedBag,
    string CheckedWeight,
    string UncheckedWeight,
    string Unit,
    string LengthOfBag,
    string WidthOfBag,
    string HeightOfBag,
    string BaggageTypeCode);

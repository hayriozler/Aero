using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Dtos;

public sealed record ArrivalBagDto(
    long Id,
    string TagNumber,
    ArrivalBaggageStatus ArrivalBaggageStatus,
    BaggageClass Class,
    int? PassengerId,
    bool IsTransfer
    );
using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Requests;

public sealed record CheckInBagRequest(
    string TagNumber,
    string FlightKey,
    decimal WeightKg,
    BaggageClass Class,
    int? PassengerId,
    string OperatorId,
    string DeviceId,
    string LocationCode);

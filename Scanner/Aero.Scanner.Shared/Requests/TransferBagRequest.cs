namespace Aero.Scanner.Shared.Requests;

public sealed record TransferBagRequest(
    string TagNumber,
    string TargetFlightKey,
    string OperatorId);

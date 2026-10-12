using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Requests;

public sealed record OffloadBagRequest(
    OffloadReason Reason,
    string OperatorId,
    string DeviceId,
    string LocationCode);

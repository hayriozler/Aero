using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Requests;

public sealed record SubmitScanRequest(
    string DeviceId,
    string TagNumber,
    string LocationCode,
    ScanType ScanType,
    string FlightKey,
    DateTime DeviceTimestamp,
    string OperatorId,
    bool IsOffline,
    double? Latitude = null,
    double? Longitude = null);

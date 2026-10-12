namespace Aero.Scanner.Shared.Requests;

public sealed record CreateContainerRequest(
    int FlightId,
    string ContainerCode,
    string ContainerTypeCode,
    string ContainerStatusCode,
    string ContainerClassCode,
    string ContainerDestination);

public sealed record UpdateContainerRequest(
    string ContainerCode,
    string ContainerTypeCode,
    string ContainerStatusCode,
    string ContainerClassCode,
    string ContainerDestination);

public sealed record CreateContainerTypeRequest(
    string Code,
    string Description,
    bool IsAllDestination,
    bool IsTransfer);

public sealed record UpdateContainerTypeRequest(
    string Description,
    bool IsAllDestination,
    bool IsTransfer);

public sealed record CreateContainerStatusRequest(
    string Code,
    string Description);

public sealed record UpdateContainerStatusRequest(
    string Description);

public sealed record CreateContainerClassRequest(
    string ContainerTypeCode,
    string ClassCode,
    string Description);

public sealed record UpdateContainerClassRequest(
    string Description);

public sealed record CreateAirlineClassMapRequest(
    string AirlineCode,
    char SourceClass,
    char TargetClass);

public sealed record UpdateAirlineClassMapRequest(
    char TargetClass);

public sealed record CreateResourceStatusMapRequest(
    string SourceResourceName,
    string SourceResourceStatus,
    string TargetResourceStatus);

public sealed record UpdateResourceStatusMapRequest(
    string TargetResourceStatus);

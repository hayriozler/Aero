namespace Aero.Scanner.Shared.Dtos;

public sealed record ContainerDto(
    int Id,
    int FlightId,
    string ContainerCode,
    string ContainerTypeCode,
    string ContainerStatusCode,
    string ContainerClassCode,
    string ContainerDestination);

public sealed record ContainerTypeDto(
    string Code,
    string Description,
    bool IsAllDestination,
    bool IsTransfer);

public sealed record ContainerClassDto(
    string ContainerTypeCode,
    string ClassCode,
    string? Description);

public sealed record AirlineClassMapDto(
    string AirlineCode,
    char SourceClass,
    char TargetClass);

public sealed record ResourceStatusMapDto(
    string SourceResourceName,
    string SourceResourceStatus,
    string TargetResourceStatus);

namespace Aero.Fids.Application.FieldMappings;

public sealed record FieldMappingResponse(
    long Id, string TargetField, string? FlightType, string SourcePath, string? RuleOperator, string? RuleValue);

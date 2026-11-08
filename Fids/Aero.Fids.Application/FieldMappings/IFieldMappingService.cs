namespace Aero.Fids.Application.FieldMappings;

public interface IFieldMappingService
{
    Task<IReadOnlyList<FieldMappingResponse>> ListAsync(CancellationToken ct);

    Task<FieldMappingResponse> SetAsync(string targetField, string? flightType, string sourcePath, CancellationToken ct);
    Task<bool> DeleteAsync(string targetField, string? flightType, CancellationToken ct);

    Task<FieldMappingResponse> SetRuleAsync(string targetField, string? flightType, string? ruleOperator, string? ruleValue, CancellationToken ct);
}

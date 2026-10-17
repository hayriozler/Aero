using Aero.Fids.Infrastructure.Flights;
using Aero.Fids.Infrastructure.Persistence;
using Aero.Fids.Web.Boards;
using Aero.Shared.Application.Tenancy;

namespace Aero.Fids.Web.Services;

public sealed record FieldMappingResponse(
    long Id, string TargetField, string? FlightType, string SourcePath, string? RuleOperator, string? RuleValue);

public interface IFieldMappingService
{
    Task<IReadOnlyList<FieldMappingResponse>> ListAsync(CancellationToken ct);

    Task<FieldMappingResponse> SetAsync(string targetField, string? flightType, string sourcePath, CancellationToken ct);
    Task<bool> DeleteAsync(string targetField, string? flightType, CancellationToken ct);

    Task<FieldMappingResponse> SetRuleAsync(string targetField, string? flightType, string? ruleOperator, string? ruleValue, CancellationToken ct);
}

internal sealed class FieldMappingService(FidsDbContext db, IFieldMappingRegistry registry, ITenantContext tenant) : IFieldMappingService
{
    private static readonly IReadOnlyList<string> _ruleableTargets =
    [
        FidsFlightFields.GateStatuses, FidsFlightFields.BeltStatuses, FidsFlightFields.CounterStatuses, FidsFlightFields.SpotStatuses,
    ];

    public async Task<IReadOnlyList<FieldMappingResponse>> ListAsync(CancellationToken ct) =>
        (await db.FieldMappingSet.AsNoTracking().OrderBy(m => m.TargetField).ThenBy(m => m.FlightType).ToListAsync(ct))
            .Select(ToResponse).ToList();

    public async Task<FieldMappingResponse> SetAsync(string targetField, string? flightType, string sourcePath, CancellationToken ct)
    {
        if (!FidsFlightFields.All.Contains(targetField))
            throw new FidsValidationException($"TargetField must be one of: {string.Join(", ", FidsFlightFields.All)}.");

        if (flightType is not (null or "Arrival" or "Departure"))
            throw new FidsValidationException("FlightType must be 'Arrival', 'Departure', or omitted.");

        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new FidsValidationException("SourcePath is required.");

        var existing = await db.FieldMappingSet.FirstOrDefaultAsync(m => m.TargetField == targetField && m.FlightType == flightType, ct);
        if (existing is null)
        {
            existing = FieldMapping.Create(targetField, flightType, sourcePath);
            db.FieldMappingSet.Add(existing);
        }
        else
        {
            existing.Update(sourcePath);
        }

        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return ToResponse(existing);
    }

    public async Task<bool> DeleteAsync(string targetField, string? flightType, CancellationToken ct)
    {
        var existing = await db.FieldMappingSet.FirstOrDefaultAsync(m => m.TargetField == targetField && m.FlightType == flightType, ct);
        if (existing is null)
            return false;

        db.FieldMappingSet.Remove(existing);
        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return true;
    }

    public async Task<FieldMappingResponse> SetRuleAsync(string targetField, string? flightType, string? ruleOperator, string? ruleValue, CancellationToken ct)
    {
        if (!_ruleableTargets.Contains(targetField))
            throw new FidsValidationException($"Rule can only be set on: {string.Join(", ", _ruleableTargets)}.");

        if (ruleOperator is not null && !FieldMappingRuleOperators.All.Contains(ruleOperator))
            throw new FidsValidationException($"RuleOperator must be one of: {string.Join(", ", FieldMappingRuleOperators.All)}.");

        if (ruleOperator is not null && string.IsNullOrWhiteSpace(ruleValue))
            throw new FidsValidationException("RuleValue is required when RuleOperator is set.");

        var row = await db.FieldMappingSet.FirstOrDefaultAsync(m => m.TargetField == targetField && m.FlightType == flightType, ct)
            ?? throw new FidsValidationException($"No mapping exists yet for '{targetField}'; set its Source Path first.");

        row.SetRule(ruleOperator, ruleOperator is null ? null : ruleValue);
        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return ToResponse(row);
    }

    private static FieldMappingResponse ToResponse(FieldMapping m) => new(m.Id, m.TargetField, m.FlightType, m.SourcePath, m.RuleOperator, m.RuleValue);
}

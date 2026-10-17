using Aero.Fids.Infrastructure.Flights;
using Aero.Fids.Infrastructure.Persistence;
using Aero.Fids.Web.Boards;
using Aero.Shared.Application.Tenancy;

namespace Aero.Fids.Web.Services;

public sealed record StatusMappingResponse(long Id, string Dimension, string? FlightType, string ExternalValue, string InternalValue);

public interface IStatusMappingService
{
    Task<IReadOnlyList<StatusMappingResponse>> ListAsync(CancellationToken ct);

    // Upserts by (Dimension, FlightType, ExternalValue). FlightType only applies to Dimension=Flight
    // (null there covers both directions; "Arrival"/"Departure" overrides just one).
    Task<StatusMappingResponse> SetAsync(string dimension, string? flightType, string externalValue, string internalValue, CancellationToken ct);
    Task<bool> DeleteAsync(string dimension, string? flightType, string externalValue, CancellationToken ct);
}

internal sealed class StatusMappingService(FidsDbContext db, IStatusMappingRegistry registry, ITenantContext tenant) : IStatusMappingService
{
    public async Task<IReadOnlyList<StatusMappingResponse>> ListAsync(CancellationToken ct) =>
        (await db.StatusMappingSet.AsNoTracking()
            .OrderBy(m => m.Dimension).ThenBy(m => m.FlightType).ThenBy(m => m.ExternalValue)
            .ToListAsync(ct))
            .Select(ToResponse).ToList();

    public async Task<StatusMappingResponse> SetAsync(string dimension, string? flightType, string externalValue, string internalValue, CancellationToken ct)
    {
        if (!StatusMappingDimensions.All.Contains(dimension))
            throw new FidsValidationException($"Dimension must be one of: {string.Join(", ", StatusMappingDimensions.All)}.");

        if (dimension != StatusMappingDimensions.Flight && flightType is not null)
            throw new FidsValidationException("FlightType only applies to the Flight dimension.");

        if (flightType is not (null or "Arrival" or "Departure"))
            throw new FidsValidationException("FlightType must be 'Arrival', 'Departure', or omitted.");

        if (string.IsNullOrWhiteSpace(externalValue))
            throw new FidsValidationException("ExternalValue is required.");

        var validInternalValues = dimension == StatusMappingDimensions.Flight ? InternalFlightStatuses.All : InternalResourceStatuses.All;
        if (!validInternalValues.Contains(internalValue))
            throw new FidsValidationException($"InternalValue must be one of: {string.Join(", ", validInternalValues)}.");

        var existing = await db.StatusMappingSet.FirstOrDefaultAsync(
            m => m.Dimension == dimension && m.FlightType == flightType && m.ExternalValue == externalValue, ct);
        if (existing is null)
        {
            existing = StatusMapping.Create(dimension, flightType, externalValue, internalValue);
            db.StatusMappingSet.Add(existing);
        }
        else
        {
            existing.Update(internalValue);
        }

        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return ToResponse(existing);
    }

    public async Task<bool> DeleteAsync(string dimension, string? flightType, string externalValue, CancellationToken ct)
    {
        var existing = await db.StatusMappingSet.FirstOrDefaultAsync(
            m => m.Dimension == dimension && m.FlightType == flightType && m.ExternalValue == externalValue, ct);
        if (existing is null)
            return false;

        db.StatusMappingSet.Remove(existing);
        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return true;
    }

    private static StatusMappingResponse ToResponse(StatusMapping m) => new(m.Id, m.Dimension, m.FlightType, m.ExternalValue, m.InternalValue);
}

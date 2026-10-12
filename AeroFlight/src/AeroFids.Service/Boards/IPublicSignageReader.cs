using AeroFids.Infrastructure.Persistence;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Service.Boards;

public sealed record ActiveSignageItem(long Id, string Name, int DisplayOrder);

public interface IPublicSignageReader
{
    Task<IReadOnlyList<ActiveSignageItem>> GetActiveAsync(string tenantId, CancellationToken ct);
    Task<(byte[] Data, string ContentType)?> GetImageAsync(string tenantId, long id, CancellationToken ct);
}

// Anonymous display boards have no cookie/claim to resolve ITenantContext from, so — like
// DisplayClientRegistry/FieldMappingRegistry — this opens its own FidsDbContext per tenant rather
// than using the scoped one. Reads straight from the DB each call; signage is polled every few
// minutes by a handful of boards at most, not worth caching.
internal sealed class PublicSignageReader(IFidsDbContextFactory dbFactory, IInterfaceSettingsService interfaceSettings) : IPublicSignageReader
{
    public async Task<IReadOnlyList<ActiveSignageItem>> GetActiveAsync(string tenantId, CancellationToken ct)
    {
        await using var db = await CreateDbAsync(tenantId, ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await db.SignageImageSet.AsNoTracking()
            .Where(s => s.IsActive && (s.StartDate == null || s.StartDate <= today) && (s.EndDate == null || s.EndDate >= today))
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new ActiveSignageItem(s.Id, s.Name, s.DisplayOrder))
            .ToListAsync(ct);
    }

    public async Task<(byte[] Data, string ContentType)?> GetImageAsync(string tenantId, long id, CancellationToken ct)
    {
        await using var db = await CreateDbAsync(tenantId, ct);
        var image = await db.SignageImageSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);
        return image is null ? null : (image.ImageData, image.ContentType);
    }

    private async Task<FidsDbContext> CreateDbAsync(string tenantId, CancellationToken ct)
    {
        var connectionString = await interfaceSettings.FindAsync(tenantId, InterfaceSettingKeys.FidsConnectionString, ct)
            ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenantId}'.");
        return dbFactory.Create(connectionString);
    }
}

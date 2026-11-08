using Aero.Fids.Domain.Signages;
using Aero.Fids.Infrastructure.Persistence;
using Aero.Shared.Application.Managers;
using Aero.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aero.Fids.Web.Boards;

internal sealed class PublicSignageReader(IFidsDbContextFactory dbFactory, IInterfaceSettingsManager interfaceSettings) : IPublicSignageReader
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

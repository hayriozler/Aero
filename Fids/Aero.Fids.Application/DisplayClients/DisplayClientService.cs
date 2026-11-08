using Aero.Fids.Domain.Boards;
using Aero.Fids.Infrastructure.Persistence;
using Aero.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Aero.Fids.Application.DisplayClients;

internal sealed class DisplayClientService(FidsDbContext db, IDisplayClientRegistry registry, ITenantContext tenant) : IDisplayClientService
{
    public async Task<IReadOnlyList<DisplayClientResponse>> ListAsync(CancellationToken ct) =>
        (await db.DisplayClientSet.AsNoTracking().OrderBy(d => d.Name).ToListAsync(ct)).Select(ToResponse).ToList();

    public async Task<DisplayClientResponse> CreateAsync(DisplayClientRequest request, CancellationToken ct)
    {
        await ValidateAsync(request, null, ct);

        var created = DisplayClient.Create(
            request.Name, request.Type, request.ResourceCode, request.Location, request.BankName, request.BankPosition, DateTimeOffset.UtcNow);
        db.DisplayClientSet.Add(created);
        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return ToResponse(created);
    }

    public async Task<DisplayClientResponse?> UpdateAsync(long id, DisplayClientRequest request, CancellationToken ct)
    {
        await ValidateAsync(request, id, ct);

        var existing = await db.DisplayClientSet.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (existing is null)
            return null;

        existing.Update(
            request.Name, request.Type, request.ResourceCode, request.Location, request.BankName, request.BankPosition,
            request.IsActive, DateTimeOffset.UtcNow);
        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return ToResponse(existing);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var existing = await db.DisplayClientSet.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (existing is null)
            return false;

        db.DisplayClientSet.Remove(existing);
        await db.SaveChangesAsync(ct);
        registry.Invalidate(tenant.TenantId);

        return true;
    }

    private async Task ValidateAsync(DisplayClientRequest request, long? excludingId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new FidsValidationException("Name is required.");

        if (!DisplayClientTypes.All.Contains(request.Type))
            throw new FidsValidationException($"Type must be one of: {string.Join(", ", DisplayClientTypes.All)}.");

        if (await db.DisplayClientSet.AnyAsync(d => d.Name == request.Name && d.Id != excludingId, ct))
            throw new FidsValidationException($"A display client named '{request.Name}' already exists.");
    }

    private static DisplayClientResponse ToResponse(DisplayClient d) =>
        new(d.Id, d.Name, d.Type, d.ResourceCode, d.Location, d.BankName, d.BankPosition, d.IsActive);
}

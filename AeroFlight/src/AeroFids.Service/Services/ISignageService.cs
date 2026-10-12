using AeroFids.Infrastructure.Persistence;
using AeroFids.Infrastructure.Signage;
using Microsoft.EntityFrameworkCore;

namespace AeroFids.Service.Services;

public sealed record SignageImageResponse(
    long Id, string Name, DateOnly? StartDate, DateOnly? EndDate, int DisplayOrder, bool IsActive, string ContentType, int ImageSizeBytes);

public interface ISignageService
{
    Task<IReadOnlyList<SignageImageResponse>> ListAsync(CancellationToken ct);
    Task<SignageImageResponse> CreateAsync(string name, byte[] data, string contentType, DateOnly? startDate, DateOnly? endDate, int displayOrder, CancellationToken ct);
    Task<SignageImageResponse?> UpdateAsync(long id, string name, DateOnly? startDate, DateOnly? endDate, int displayOrder, bool isActive, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}

internal sealed class SignageService(FidsDbContext db) : ISignageService
{
    public async Task<IReadOnlyList<SignageImageResponse>> ListAsync(CancellationToken ct) =>
        (await db.SignageImageSet.AsNoTracking().OrderBy(s => s.DisplayOrder).ToListAsync(ct)).Select(ToResponse).ToList();

    public async Task<SignageImageResponse> CreateAsync(
        string name, byte[] data, string contentType, DateOnly? startDate, DateOnly? endDate, int displayOrder, CancellationToken ct)
    {
        Validate(name, data, startDate, endDate);

        var image = SignageImage.Create(name, data, contentType, startDate, endDate, displayOrder);
        db.SignageImageSet.Add(image);
        await db.SaveChangesAsync(ct);
        return ToResponse(image);
    }

    public async Task<SignageImageResponse?> UpdateAsync(
        long id, string name, DateOnly? startDate, DateOnly? endDate, int displayOrder, bool isActive, CancellationToken ct)
    {
        var image = await db.SignageImageSet.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (image is null)
            return null;

        Validate(name, [1], startDate, endDate);
        image.Update(name, startDate, endDate, displayOrder, isActive);
        await db.SaveChangesAsync(ct);
        return ToResponse(image);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var image = await db.SignageImageSet.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (image is null)
            return false;

        db.SignageImageSet.Remove(image);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static void Validate(string name, byte[] data, DateOnly? startDate, DateOnly? endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new FidsValidationException("Name is required.");
        if (data.Length == 0)
            throw new FidsValidationException("Image data is required.");
        if (startDate is { } s && endDate is { } e && s > e)
            throw new FidsValidationException("Start date must be on or before end date.");
    }

    private static SignageImageResponse ToResponse(SignageImage s) =>
        new(s.Id, s.Name, s.StartDate, s.EndDate, s.DisplayOrder, s.IsActive, s.ContentType, s.ImageData.Length);
}

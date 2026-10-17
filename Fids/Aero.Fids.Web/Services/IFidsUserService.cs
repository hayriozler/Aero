using Aero.Fids.Infrastructure.Identity;
using Aero.Fids.Infrastructure.Persistence;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Fids.Web.Services;

public sealed record CreateFidsUserRequest(string Username, string Password, string Role);
public sealed record UpdateFidsUserRequest(string Role, bool IsActive, string? NewPassword);
public sealed record FidsUserResponse(int Id, string Username, string Role, bool IsActive);

public interface IFidsUserService
{
    Task<IReadOnlyList<FidsUserResponse>> ListAsync(CancellationToken ct);
    Task<FidsUserResponse> CreateAsync(CreateFidsUserRequest request, CancellationToken ct);
    Task<FidsUserResponse?> UpdateAsync(int id, UpdateFidsUserRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

internal sealed class FidsUserService(FidsDbContext db, PasswordHasherService passwordHasher) : IFidsUserService
{
    public async Task<IReadOnlyList<FidsUserResponse>> ListAsync(CancellationToken ct) =>
        (await db.FidsUserSet.AsNoTracking().OrderBy(u => u.Username).ToListAsync(ct)).Select(ToResponse).ToList();

    public async Task<FidsUserResponse> CreateAsync(CreateFidsUserRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new FidsValidationException("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new FidsValidationException("Password must be at least 6 characters.");

        if (await db.FidsUserSet.AnyAsync(u => u.Username == request.Username, ct))
            throw new FidsValidationException($"Username '{request.Username}' is already taken.");

        var role = string.IsNullOrWhiteSpace(request.Role) ? FidsRoles.Admin : request.Role;
        var created = FidsUser.Create(request.Username, passwordHasher.Hash(request.Password), role, DateTimeOffset.UtcNow);
        db.FidsUserSet.Add(created);
        await db.SaveChangesAsync(ct);

        return ToResponse(created);
    }

    public async Task<FidsUserResponse?> UpdateAsync(int id, UpdateFidsUserRequest request, CancellationToken ct)
    {
        var existing = await db.FidsUserSet.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (existing is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (request.NewPassword.Length < 6)
                throw new FidsValidationException("Password must be at least 6 characters.");

            existing.SetPassword(passwordHasher.Hash(request.NewPassword));
        }

        existing.Update(request.Role, request.IsActive);
        await db.SaveChangesAsync(ct);

        return ToResponse(existing);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var existing = await db.FidsUserSet.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (existing is null)
            return false;

        db.FidsUserSet.Remove(existing);
        await db.SaveChangesAsync(ct);

        return true;
    }

    private static FidsUserResponse ToResponse(FidsUser u) => new(u.Id, u.Username, u.Role, u.IsActive);
}

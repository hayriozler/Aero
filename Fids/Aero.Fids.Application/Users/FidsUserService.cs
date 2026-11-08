using Aero.Fids.Infrastructure.Persistence;
using Aero.Shared.Domain.Consts;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Aero.Fids.Application.Users;

internal sealed class FidsUserService(FidsDbContext db, IPasswordHasherService passwordHasher) : IFidsUserService
{
    public async Task<IReadOnlyList<FidsUserResponse>> ListAsync(CancellationToken ct) =>
        (await db.UserSet.AsNoTracking().OrderBy(u => u.Username).ToListAsync(ct)).Select(ToResponse).ToList();

    public async Task<FidsUserResponse> CreateAsync(CreateFidsUserRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new FidsValidationException("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new FidsValidationException("Password must be at least 6 characters.");

        if (await db.UserSet.AnyAsync(u => u.Username == request.Username, ct))
            throw new FidsValidationException($"Username '{request.Username}' is already taken.");

        var role = string.IsNullOrWhiteSpace(request.Role) ? FixedRole.Admin : request.Role;
        var created = User.Create(request.Username, passwordHasher.Hash(request.Password), role, DateTimeOffset.UtcNow);
        db.UserSet.Add(created);
        await db.SaveChangesAsync(ct);

        return ToResponse(created);
    }

    public async Task<FidsUserResponse?> UpdateAsync(int id, UpdateFidsUserRequest request, CancellationToken ct)
    {
        var existing = await db.UserSet.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (existing is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (request.NewPassword.Length < 6)
                throw new FidsValidationException("Password must be at least 6 characters.");

            existing.UpdatePassword(passwordHasher.Hash(request.NewPassword));
        }

        existing.AssignRole(request.Role);
        await db.SaveChangesAsync(ct);

        return ToResponse(existing);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var existing = await db.UserSet.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (existing is null)
            return false;

        db.UserSet.Remove(existing);
        await db.SaveChangesAsync(ct);

        return true;
    }

    private static FidsUserResponse ToResponse(User u) => new(u.Id, u.Username, u.Roles.Select(s => s.RoleName).ToList().AsReadOnly(), u.IsActive);
}

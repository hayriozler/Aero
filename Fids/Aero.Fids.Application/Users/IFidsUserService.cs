namespace Aero.Fids.Application.Users;

public interface IFidsUserService
{
    Task<IReadOnlyList<FidsUserResponse>> ListAsync(CancellationToken ct);
    Task<FidsUserResponse> CreateAsync(CreateFidsUserRequest request, CancellationToken ct);
    Task<FidsUserResponse?> UpdateAsync(int id, UpdateFidsUserRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

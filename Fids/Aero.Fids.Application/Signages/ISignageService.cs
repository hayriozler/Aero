namespace Aero.Fids.Application.Signages;

public interface ISignageService
{
    Task<IReadOnlyList<SignageImageResponse>> ListAsync(CancellationToken ct);
    Task<SignageImageResponse> CreateAsync(string name, byte[] data, string contentType, DateOnly? startDate, DateOnly? endDate, int displayOrder, CancellationToken ct);
    Task<SignageImageResponse?> UpdateAsync(long id, string name, DateOnly? startDate, DateOnly? endDate, int displayOrder, bool isActive, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}

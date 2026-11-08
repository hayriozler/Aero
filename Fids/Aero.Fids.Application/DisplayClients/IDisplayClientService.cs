namespace Aero.Fids.Application.DisplayClients;

public interface IDisplayClientService
{
    Task<IReadOnlyList<DisplayClientResponse>> ListAsync(CancellationToken ct);
    Task<DisplayClientResponse> CreateAsync(DisplayClientRequest request, CancellationToken ct);
    Task<DisplayClientResponse?> UpdateAsync(long id, DisplayClientRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}

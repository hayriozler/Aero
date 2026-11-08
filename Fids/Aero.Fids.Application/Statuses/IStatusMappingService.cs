namespace Aero.Fids.Application.Statuses;

public interface IStatusMappingService
{
    Task<IReadOnlyList<StatusMappingResponse>> ListAsync(CancellationToken ct);

    Task<StatusMappingResponse> SetAsync(string dimension, string? flightType, string externalValue, string internalValue, CancellationToken ct);
    Task<bool> DeleteAsync(string dimension, string? flightType, string externalValue, CancellationToken ct);
}

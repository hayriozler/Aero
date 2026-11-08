using Aero.Fids.Domain.Signages;

namespace Aero.Fids.Web.Boards;



public interface IPublicSignageReader
{
    Task<IReadOnlyList<ActiveSignageItem>> GetActiveAsync(string tenantId, CancellationToken ct);
    Task<(byte[] Data, string ContentType)?> GetImageAsync(string tenantId, long id, CancellationToken ct);
}

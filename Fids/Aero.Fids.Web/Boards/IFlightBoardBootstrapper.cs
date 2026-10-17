namespace Aero.Fids.Web.Boards;

public interface IFlightBoardBootstrapper
{
    Task EnsureBootstrappedAsync(string tenantId, string flightType, CancellationToken ct);
}

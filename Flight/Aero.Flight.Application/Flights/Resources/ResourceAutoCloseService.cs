using Aero.Flights.Application.Configuration;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.Resources;

public interface IResourceAutoCloseService
{
    Task CloseAllIfCompletedAsync(ArrivalFlight flight, CancellationToken ct);
    Task CloseAllIfCompletedAsync(DepartureFlight flight, CancellationToken ct);
}

// Settings-driven, not the Business Rules Engine — Resources.AutoCloseOnCompletion.* in AeroConfig
// (see AeroConfigSeeder) decides whether this runs at all and which status code counts as
// "completed". Runs from every flight status write path (FlightUpdateService, UpdateFlightStatusCommandHandler).
public sealed class ResourceAutoCloseService(IAeroFlightDbContext db, IAeroConfigService aeroConfig, IResourceDefaultStatusResolver statusResolver, TimeProvider clock)
    : IResourceAutoCloseService
{
    public async Task CloseAllIfCompletedAsync(ArrivalFlight flight, CancellationToken ct)
    {
        if (!await IsCompletedAsync(flight.Status, ct))
            return;

        await CloseKindAsync<OpsArrivalGate>(flight.Id, ct);
        await CloseKindAsync<OpsArrivalBelt>(flight.Id, ct);
    }

    public async Task CloseAllIfCompletedAsync(DepartureFlight flight, CancellationToken ct)
    {
        if (!await IsCompletedAsync(flight.Status, ct))
            return;

        await CloseKindAsync<OpsDepartureGate>(flight.Id, ct);
        await CloseKindAsync<OpsDepartureBelt>(flight.Id, ct);
        await CloseKindAsync<OpsCounter>(flight.Id, ct);
    }

    private async Task<bool> IsCompletedAsync(string? statusCode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(statusCode))
            return false;

        var enabled = await aeroConfig.GetAsync("Resources.AutoCloseOnCompletion.Enabled", ct);
        if (!string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase))
            return false;

        var completedCode = await aeroConfig.GetAsync("Resources.AutoCloseOnCompletion.StatusCode", ct);
        return !string.IsNullOrWhiteSpace(completedCode) && string.Equals(statusCode, completedCode, StringComparison.OrdinalIgnoreCase);
    }

    private async Task CloseKindAsync<TResource>(long flightId, CancellationToken ct) where TResource : ResourceBase
    {
        var closeCode = await statusResolver.ResolveCloseStatusCodeAsync<TResource>(ct);
        if (closeCode is null)
            return;

        var now = clock.GetUtcNow();
        var resources = await db.Query<TResource>().Where(r => r.FlightId == flightId).ToListAsync(ct);
        foreach (var resource in resources)
        {
            if (string.Equals(resource.StatusCode, closeCode, StringComparison.OrdinalIgnoreCase))
                continue;

            resource.UpdateStatus(closeCode);
            resource.RecordActual(resource.BeginActual, now);
        }
    }
}

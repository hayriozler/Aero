using AeroFlight.Application.Tenancy;
using AeroFlight.Contracts.Fids;

namespace AeroFids.Service.Boards;

// Runs once at startup so a tenant's board is already populated before its first display client
// ever connects — without this, the store stays empty until some client's own snapshot fetch
// happens to trigger FlightBoardBootstrapper (see BoardEndpoints), which still works but means the
// very first connect after a restart pays for the bootstrap instead of finding a warm board.
internal sealed class EagerBoardBootstrapper(
    IServiceScopeFactory scopeFactory, ITenantStore tenantStore, ILogger<EagerBoardBootstrapper> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Every active tenant is attempted — FlightBoardBootstrapper itself skips (with a log) any
        // tenant that has no "AeroFids" ClientApp provisioned yet. A client can subscribe to either
        // direction for any resource kind, so both are always warmed.
        var tenants = await tenantStore.GetActiveAsync(stoppingToken);
        foreach (var tenant in tenants)
        {
            foreach (var flightType in new[] { FlightTypes.Arrival, FlightTypes.Departure })
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var bootstrapper = scope.ServiceProvider.GetRequiredService<IFlightBoardBootstrapper>();
                    await bootstrapper.EnsureBootstrappedAsync(tenant.Id, flightType, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Eager bootstrap failed for tenant {TenantId}, flight type {FlightType}.", tenant.Id, flightType);
                }
            }
        }
    }
}

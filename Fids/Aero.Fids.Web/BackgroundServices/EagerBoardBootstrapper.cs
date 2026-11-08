using Aero.Fids.Web.Boards;
using Aero.Shared.Application.Tenancy;

namespace Aero.Fids.Web.BackgroundServices;

internal sealed class EagerBoardBootstrapper(
    IServiceScopeFactory scopeFactory, ITenantStore tenantStore, ILogger<EagerBoardBootstrapper> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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

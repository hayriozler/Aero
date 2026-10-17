using Aero.Shared.Application.Managers;
using Aero.Shared.Application.Tenancy;
using Aero.Shared.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aero.Shared.WebUtilities.DependencyInjections;

public static class TenantCatalogServiceCollectionExtensions
{
    public static IServiceCollection AddTenant(
        this IServiceCollection services, string tenantConnectionString, string encryptionKeyBase64)
    {
        var key = Convert.FromBase64String(encryptionKeyBase64);
        if (key.Length != 32)
            throw new InvalidOperationException("Tenancy encryption key must be a base64-encoded 32-byte (256-bit) key.");

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<IStringProtector>(new ConnectionStringProtectorService(key));
        services.AddDbContextFactory<TenantDbContext>(options =>
            options.UseNpgsql(tenantConnectionString, npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations")));
        services.AddSingleton<ITenantStore, TenantStore>();
        services.AddSingleton<ITenantProvisioner, TenantProvisioner>();
        services.AddScoped<IClientAppManager, ClientAppManager>();
        services.AddSingleton<IInterfaceSettingsManager, InterfaceSettingsManager>();

        return services;
    }
}

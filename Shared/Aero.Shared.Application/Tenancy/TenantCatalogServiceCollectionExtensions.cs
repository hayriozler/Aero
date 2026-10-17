using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aero.Shared.Application.Tenancy;

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
        services.AddScoped<IClientAppService, ClientAppService>();
        services.AddSingleton<IInterfaceSettingsService, InterfaceSettingsService>();

        return services;
    }
}

using AeroFids.Infrastructure.Persistence;
using AeroFlight.Application.Configuration;
using AeroFlight.Application.Flights.Resources;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Rules;
using AeroFlight.Application.Tenancy;
using AeroFlight.Domain.Tenancy;
using AeroFlight.Infrastructure.Identity;
using AeroFlight.Infrastructure.Persistence;
using AeroFlight.Infrastructure.Tenancy;
using AeroFlight.Migrations.Options;
using AeroFlight.Migrations.Seeders;
using AeroFlight.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.AddServiceDefaults();

var tenantConnectionString = builder.Configuration.GetConnectionString("tenant")
    ?? throw new InvalidOperationException("Connection string 'tenant' is not configured.");
var tenancyEncryptionKey = builder.Configuration["Tenancy:EncryptionKey"]
    ?? throw new InvalidOperationException("Tenancy:EncryptionKey is not configured.");

builder.Services.AddTenant(tenantConnectionString, tenancyEncryptionKey);

// Tenant-scoped AeroFlightDbContext; the connection is set per tenant in the loop below.
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddSingleton<IFidsDbContextFactory, FidsDbContextFactory>();
builder.Services.AddSingleton<IAeroFlightDbContextFactory, AeroFlightDbContextFactory>();
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IAeroFlightDbContextFactory>().Create(
        AeroFlightConnectionStrings.ForTenant(tenantConnectionString, sp.GetRequiredService<ITenantContext>().TenantId))
    );


builder.Services.AddScoped(sp =>
{
    var tenantId = sp.GetRequiredService<ITenantContext>().TenantId;
    var connectionString = sp.GetRequiredService<IInterfaceSettingsService>()
        .FindAsync(tenantId, InterfaceSettingKeys.FidsConnectionString).GetAwaiter().GetResult()
        ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenantId}'.");
    return sp.GetRequiredService<IFidsDbContextFactory>().Create(connectionString);
});
builder.Services.AddScoped<FidsIdentitySeeder>();
builder.Services.AddScoped<FieldMappingSeeder>();
builder.Services.AddScoped<StatusMappingSeeder>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAeroFlightDbContext>(sp => sp.GetRequiredService<AeroFlightDbContext>());
builder.Services.AddScoped<IAeroConfigService, AeroConfigService>();
builder.Services.AddScoped<IResourceDefaultStatusResolver, ResourceDefaultStatusResolver>();

builder.Services.Configure<SeedOptions>(builder.Configuration.GetSection(SeedOptions.Section));
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IdentitySeeder>();
builder.Services.AddScoped<ClientAppSeeder>();
builder.Services.AddScoped<ResourceSeeder>();
builder.Services.AddScoped<JobScheduleSeeder>();
builder.Services.AddScoped<AeroConfigSeeder>();
builder.Services.AddSingleton<FlightRuleFieldCatalog>();
builder.Services.AddScoped<RuleCatalogSeeder>();
#if DEBUG
builder.Services.AddScoped<TestRulesSeeder>();
builder.Services.AddScoped<FlightSeeder>();
builder.Services.AddScoped<DemoDisplayClientSeeder>();
#endif

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Applying catalog migrations...");
var tenantFactory = host.Services.GetRequiredService<IDbContextFactory<TenantDbContext>>();
await using (var tenant = await tenantFactory.CreateDbContextAsync())
    await tenant.Database.MigrateAsync();

var provisioner = host.Services.GetRequiredService<ITenantProvisioner>();

// Retire the old 'dev' tenant so background services stop trying to reach its (deleted) DB.
await using (var tenant = await tenantFactory.CreateDbContextAsync())
    await tenant.TenantSet.Where(t => t.Id == "dev").ExecuteDeleteAsync();

// Tenant DB is created by MigrateAsync in the loop below if it does not exist.
await provisioner.UpsertAsync("ayt", "AYT AODB", TenantConnectionString("ayt_aodb"), "AYT");
await provisioner.UpsertAsync("saw", "SAW AODB", TenantConnectionString("saw_aodb"), "SAW");

string TenantConnectionString(string database) =>
    new NpgsqlConnectionStringBuilder(tenantConnectionString) { Database = database }.ConnectionString;

var store = host.Services.GetRequiredService<ITenantStore>();
foreach (var tenant in await store.GetActiveAsync())
{
    await using var scope = host.Services.CreateAsyncScope();
    scope.ServiceProvider.GetRequiredService<ITenantContext>().Set(tenant);
    var db = scope.ServiceProvider.GetRequiredService<AeroFlightDbContext>();

    logger.LogInformation("Migrating and seeding tenant {TenantId}...", tenant.Id);
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<IdentitySeeder>().SeedAsync(CancellationToken.None);
    if (tenant.Id == "ayt")
    {
        await scope.ServiceProvider.GetRequiredService<ClientAppSeeder>().SeedAsync(CancellationToken.None);
    }
    await scope.ServiceProvider.GetRequiredService<ResourceSeeder>().SeedAsync(CancellationToken.None);
    await scope.ServiceProvider.GetRequiredService<JobScheduleSeeder>().SeedAsync(CancellationToken.None);
    await scope.ServiceProvider.GetRequiredService<AeroConfigSeeder>().SeedAsync(CancellationToken.None);
    await scope.ServiceProvider.GetRequiredService<RuleCatalogSeeder>().SeedAsync(CancellationToken.None);
#if DEBUG
    await scope.ServiceProvider.GetRequiredService<TestRulesSeeder>().SeedAsync(CancellationToken.None);
    await scope.ServiceProvider.GetRequiredService<FlightSeeder>().SeedAsync(CancellationToken.None);
#endif

    // FIDS's own DB connection string is stored explicitly per tenant (not derived at runtime from
    // the AODB one) — seed the conventional default once, on first sight of a tenant.
    var interfaceSettings = scope.ServiceProvider.GetRequiredService<IInterfaceSettingsService>();
    if (await interfaceSettings.FindAsync(tenant.Id, InterfaceSettingKeys.FidsConnectionString, CancellationToken.None) is null)
    {
        var fidsConnectionString = FidsConnectionStrings.ForTenant(tenantConnectionString, tenant.Id);
        await interfaceSettings.SetAsync(tenant.Id, InterfaceSettingKeys.FidsConnectionString, fidsConnectionString, CancellationToken.None);
    }

    logger.LogInformation("Migrating and seeding FIDS database for tenant {TenantId}...", tenant.Id);
    var fidsDb = scope.ServiceProvider.GetRequiredService<FidsDbContext>();
    await fidsDb.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<FidsIdentitySeeder>().SeedAsync(CancellationToken.None);
    await scope.ServiceProvider.GetRequiredService<FieldMappingSeeder>().SeedAsync(CancellationToken.None);
    await scope.ServiceProvider.GetRequiredService<StatusMappingSeeder>().SeedAsync(CancellationToken.None);
#if DEBUG
    await scope.ServiceProvider.GetRequiredService<DemoDisplayClientSeeder>().SeedAsync(CancellationToken.None);
#endif

    // Dev convenience: FIDS is enabled by default for every seeded tenant.
    await using (var catalog = await tenantFactory.CreateDbContextAsync())
    {
        var fidsInterface = await catalog.TenantInterfaceSet
            .FirstOrDefaultAsync(ti => ti.TenantId == tenant.Id && ti.InterfaceCode == InterfaceCodes.Fids);
        if (fidsInterface is null)
            catalog.TenantInterfaceSet.Add(TenantInterface.Create(tenant.Id, InterfaceCodes.Fids, true, DateTimeOffset.UtcNow));
        else
            fidsInterface.SetEnabled(true, DateTimeOffset.UtcNow);
        await catalog.SaveChangesAsync();
    }

    logger.LogInformation("Tenant {TenantId} done.", tenant.Id);
}

logger.LogInformation("All tenants migrated and seeded.");

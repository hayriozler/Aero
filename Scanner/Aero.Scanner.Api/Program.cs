using Aero.Scanner.Api.Endpoints;
using Aero.Scanner.Api.Extensions;
using Aero.Scanner.Api.Persistence;
using Aero.Scanner.Api.Services;
using Aero.Scanner.Api.Validators;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Diagnostics;
using Aero.Shared.Infrastructure.Interceptors;
using Aero.Shared.Infrastructure.Persistence;
using Aero.Shared.Infrastructure.Services;
using Aero.Shared.WebUtilities.Middlewares;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSingleton(typeof(LoggerService<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<QueryCollector>();
builder.Services.AddScoped<QueryCollectorInterceptor>();

builder.Services.Configure<SlowRequestOptions>(
    builder.Configuration.GetSection(SlowRequestOptions.Section));

builder.Services.AddDbContext<IDataContext, AeroScanDataContext>((sp, opt) =>
{
    var connectionStr = builder.Configuration.GetConnectionString("aeroscan");
    opt.UseNpgsql(connectionStr, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("Migratons", "references");
        npgsqlOptions.ExecutionStrategy(dependencies => new PostgresRetryStrategy(
            dependencies,
            sp.GetRequiredService<ILogger<PostgresRetryStrategy>>(),
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10)));
    });
    opt.AddInterceptors(
        sp.GetRequiredService<AuditInterceptor>(),
        sp.GetRequiredService<QueryCollectorInterceptor>());
#if DEBUG
    opt.EnableDetailedErrors(true);
    opt.EnableSensitiveDataLogging(true);
#endif
});

builder.Services.AddScoped<DataStoreContext>(sp => sp.GetRequiredService<AeroScanDataContext>());


builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<HandlingAssignmentService>();

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddAeroScanJwtAuth(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.AddScoped<DataSeedService>();
builder.Services.AddScoped<DataSeedFakerService>();

builder.Services.InstallMessageHandlers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeedService>();
    await seeder.SeedAsync();

    var faker = scope.ServiceProvider.GetRequiredService<DataSeedFakerService>();
    await faker.FeedDummyDataAsync(25);
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SlowRequestMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();
app.MapAuthEndpoints();
app.MapAuditEndpoints();
app.MapDepartureBagEndpoints();
app.MapArrivalFlightEndpoints();
app.MapDepartureFlightEndpoints();
app.MapDashboardEndpoints();
app.MapCompanyEndpoints();
app.MapHandlingContractEndpoints();
app.MapRoleEndpoints();
app.MapPermissionEndpoints();
app.MapUserEndpoints();
app.MapHandheldTerminalEndpoints();
app.MapContainerEndpoints();
app.MapContainerTypeEndpoints();
app.MapContainerClassEndpoints();
app.MapAirlineClassMapEndpoints();
app.MapResourceStatusMapEndpoints();
app.MapSystemConfigurationEndpoints();

await app.RunAsync();
using BaggageService.Endpoints;
using BaggageService.Extensions;
using BaggageService.Middleware;
using BaggageService.Persistence;
using BaggageService.Services;
using BaggageService.Validators;
using FluentValidation;
using IataText.Parser.Extensions;
using IataText.Parser.Persistence;
using Infrastructure.Diagnostics;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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
builder.Services.AddOpenApi();
builder.Services.AddScoped<DataStoreContext>(sp => sp.GetRequiredService<AeroScanDataContext>());
builder.Services.AddScoped<ITextParserDbContext>(sp => sp.GetRequiredService<AeroScanDataContext>());

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<HandlingAssignmentService>();

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddAeroScanJwtAuth(builder.Configuration);

builder.Services.AddScoped<DataSeedService>();
builder.Services.AddScoped<DataSeedFakerService>();

builder.Services.InstallMessages();
builder.Services.InstallElements();
builder.Services.InstallMessageDispatchers();

builder.Services.AddHostedService<MessageProcessingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeedService>();
    await seeder.SeedAsync();

    var faker = scope.ServiceProvider.GetRequiredService<DataSeedFakerService>();
    await faker.FeedDummyDataAsync(25);
}
app.UseAeroScanExceptionHandling();
app.UseMiddleware<SlowRequestMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // Expose a lightweight OpenAPI JSON document for development
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("AeroScan Baggage Service"));
}
app.MapMessagingEndpoints();
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

app.Run();


using Aero.ServiceDefaults;
using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Diagnostics;
using Aero.Shared.Infrastructure.Persistence;
using Aero.Shared.Infrastructure.Services;
using Aero.Shared.WebUtilities.Middlewares;
using Aero.TextMessageParser.Api.Endpoints;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton(typeof(LoggerService<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<QueryCollector>();
builder.Services.AddScoped<QueryCollectorInterceptor>();

builder.Services
    .InstallElements()
    .InstallMessages()
    .InstallProcessors();

builder.Services.Configure<SlowRequestOptions>(builder.Configuration.GetSection(SlowRequestOptions.Section));
builder.Services.AddDbContext<TextMessageParserDbContext>((sp, opt) =>
{
    var connectionStr = builder.Configuration.GetConnectionString("textmessageparser");
    opt.UseNpgsql(connectionStr, npgsqlOptions =>
    {
        npgsqlOptions.ExecutionStrategy(dependencies => new PostgresRetryStrategy(
            dependencies,
            sp.GetRequiredService<ILogger<PostgresRetryStrategy>>(),
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10)));
    });
    opt.AddInterceptors(sp.GetRequiredService<QueryCollectorInterceptor>());
#if DEBUG
    opt.EnableDetailedErrors(true);
    opt.EnableSensitiveDataLogging(true);
#endif
});

builder.Services.Configure<MessageProcessingOptions>(builder.Configuration.GetSection(MessageProcessingOptions.Section));
builder.Services.AddOpenApi();
builder.Services.AddHostedService<MessageProcessingService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SlowRequestMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapMessagingEndpoints();
app.MapParsedMessageEndpoints();

await app.RunAsync();
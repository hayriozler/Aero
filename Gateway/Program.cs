using Aero.ServiceDefaults;
using Aero.Shared.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var scannerJwtKey = builder.Configuration["Jwt:Scanner:Key"];
var flightJwtKey = builder.Configuration["Jwt:Flight:Key"];
var fidsJwtKey = builder.Configuration["Jwt:Fids:Key"];

if (!string.IsNullOrEmpty(scannerJwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("ScannerJwt", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidIssuer              = builder.Configuration["Jwt:Scanner:Issuer"] ?? "scanner-issuer",
                ValidateAudience         = true,
                ValidAudience            = builder.Configuration["Jwt:Scanner:Audience"] ?? "scanner-api",
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(scannerJwtKey)),
                ClockSkew                = TimeSpan.FromMinutes(1)
            };
        });
}
if (!string.IsNullOrEmpty(flightJwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("FlightJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Flight:Issuer"] ?? "flight-issuer",
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Flight:Audience"] ?? "flight-api",
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(flightJwtKey)),
            ClockSkew                = TimeSpan.FromMinutes(1)
        };
    });

}
if (!string.IsNullOrEmpty(fidsJwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("FidsJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Fids:Issuer"] ?? "fids-issuer",
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Fids:Audience"] ?? "fids-api",
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(fidsJwtKey)),
            ClockSkew                = TimeSpan.FromMinutes(1)
        };
    });

}
var authBuilder = builder.Services.AddAuthorizationBuilder();
if (string.IsNullOrEmpty(scannerJwtKey))
{
    authBuilder.AddPolicy("ScannerJwt", policy =>
    {
        policy.AddAuthenticationSchemes("ScannerJwt");
        policy.RequireAuthenticatedUser();
    });
}
if (string.IsNullOrEmpty(flightJwtKey))
{
    authBuilder.AddPolicy("FlightJwt", policy =>
    {
        policy.AddAuthenticationSchemes("FlightJwt");
        policy.RequireAuthenticatedUser();
    });
}
if (!string.IsNullOrEmpty(fidsJwtKey))
{
    authBuilder.AddPolicy("FidsJwt", policy =>
    {
        policy.AddAuthenticationSchemes("FidsJwt");
        policy.RequireAuthenticatedUser();
    });
}
if (!string.IsNullOrEmpty(fidsJwtKey))
{
    authBuilder.AddPolicy("FidsJwt", policy =>
    {
        policy.AddAuthenticationSchemes("FidsJwt");
        policy.RequireAuthenticatedUser();
    });
}

// ── Rate limiting ──────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.Window                = TimeSpan.FromMinutes(1);
        limiterOptions.SegmentsPerWindow     = 6;
        limiterOptions.PermitLimit           = 300;
        limiterOptions.QueueLimit            = 50;
        limiterOptions.QueueProcessingOrder  = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── Gateway services ──────────────────────────────────────────────────────
builder.Services.AddSingleton<LoggerService<Program>>();

// ── YARP reverse proxy ─────────────────────────────────────────────────────
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapDefaultEndpoints();

// Log every request that passes through the gateway
var gatewayLogger = app.Services.GetRequiredService<LoggerService<Program>>();

app.Use(async (ctx, next) =>
{
    var start = TimeProvider.System.GetTimestamp();
    await next(ctx);
    var elapsed = TimeProvider.System.GetElapsedTime(start);

    gatewayLogger.LogInfo(
        "[{Method}] {Path}{Query} → {StatusCode} in {ElapsedMs}ms  user={User}",
        ctx.Request.Method,
        ctx.Request.Path.ToString(),
        ctx.Request.QueryString.ToString(),
        ctx.Response.StatusCode,
        (long)elapsed.TotalMilliseconds,
        ctx.User.Identity?.Name ?? "anonymous");
});

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapReverseProxy();

app.Run();

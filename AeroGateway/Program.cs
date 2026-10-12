using Aero.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var signingKey = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"] ?? "aeroscan",
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"] ?? "aeroscan-clients",
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew                = TimeSpan.FromMinutes(1)
        };
    });

// "authenticated" policy: requires a valid JWT only — role/scope decisions are made in BaggageService
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("authenticated", policy => policy.RequireAuthenticatedUser());

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

using Aero.Scanner.WebApp.EndPoints;
using Aero.Scanner.WebApp.Services;
using Aero.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// ── Cookie Authentication ──────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        // Cookie lifetime matches refresh token; JWT inside it is 15 min and refreshed transparently
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = false;
        options.Cookie.Name = "brs.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();

// ── Localization ──────────────────────────────────────────────────────────
builder.Services.AddLocalization();

// ── Blazor ────────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        // Show real exception details in the browser console during development.
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });

// ── HTTP Client → API Gateway ─────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<UserSettingsService>();
builder.Services.AddScoped<ClientTimeService>();
builder.Services.AddMudServices();

static void ConfigureGatewayClient(HttpClient client) =>
    client.BaseAddress = new Uri("https+http://api-gateway");

// Bare client used internally for token refresh / logout — no AuthTokenHandler to avoid circular calls
builder.Services.AddHttpClient("refresh", ConfigureGatewayClient);

builder.Services.AddHttpClient<AuthApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<DashboardApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<DepartureFlightApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<ArrivalFlightApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<DepartureBagApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<HandlingContractApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<CompanyApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<RoleApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<UserApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<HhtApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<ContainerApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<AuditApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();


builder.Services.AddHttpClient<SystemConfigurationApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

// ── Antiforgery (for SSR forms) ───────────────────────────────────────────
builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async ctx =>
{
    var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
    var ex = feature?.Error;

    if (ex is OperationCanceledException && ctx.RequestAborted.IsCancellationRequested)
        return;

    var log = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
    log.LogError(ex, "Unhandled exception on {Method} {Path}", ctx.Request.Method, ctx.Request.Path);

    if (!ctx.Response.HasStarted)
    {
        ctx.Response.StatusCode  = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = "application/problem+json";
        try
        {
            await ctx.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status   = StatusCodes.Status500InternalServerError,
                Title    = "An unexpected error occurred.",
                Instance = ctx.Request.Path
            });
        }
        catch { /* best-effort — connection may already be gone */ }
    }
}));

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

// ── Request Localization — reads .AspNetCore.Culture cookie ───────────────
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("tr") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures     = supportedCultures,
    SupportedUICultures   = supportedCultures,
    RequestCultureProviders = [new CookieRequestCultureProvider()]
});

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapDefaultEndpoints();

app.MapRazorComponents<Aero.Scanner.WebApp.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

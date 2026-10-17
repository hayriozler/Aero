using Aero.Flights.Web.Endpoints.Auth;
using Aero.Flights.Web.Services;
using Aero.Flights.Web.Services.Flights;
using Aero.Flights.Web.Services.Metadatas;
using Aero.Flights.Web.Services.Rules;
using Aero.Flights.Web.Services.SeasonalFlightSchedules;
using Aero.ServiceDefaults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

builder.AddServiceDefaults();

builder.Services.AddRazorPages();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var expiresClaim = context.Principal?.FindFirst("api_token_expires")?.Value;
                if (expiresClaim is not null
                    && DateTimeOffset.TryParse(expiresClaim, out var expiresAt)
                    && expiresAt <= DateTimeOffset.UtcNow)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            },
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<IMetadataClient, MetadataDataClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<IAirlineExtrasClient, AirlineExtrasClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<IAirportImagesClient, AirportImagesClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<ICompanyExtrasClient, CompanyExtrasClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<IRulesClient, RulesClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<IFlightsClient, FlightsClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});

builder.Services.AddHttpClient<ISeasonalFlightScheduleClient, SeasonalFlightScheduleClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

// A cookie can outlive the backend JWT it carries (OnValidatePrincipal signs it out mid-request).
// If that happens right as /login renders, the antiforgery token gets minted against a user that
// no longer matches by the time the form posts. Force a clean, anonymous render instead.
app.Use(async (ctx, next) =>
{
    if (HttpMethods.IsGet(ctx.Request.Method)
        && ctx.Request.Path.Equals("/login", StringComparison.OrdinalIgnoreCase)
        && ctx.User.Identity?.IsAuthenticated == true)
    {
        await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        ctx.User = new ClaimsPrincipal(new ClaimsIdentity());
    }
    await next(ctx);
});

app.UseAuthorization();
app.UseAntiforgery();
app.MapAuthEndpoints();
app.MapRazorPages();

app.MapDefaultEndpoints();


app.MapRazorComponents<Aero.Flights.Web.App>()
    .AddInteractiveServerRenderMode()
    .WithStaticAssets();

await app.RunAsync();

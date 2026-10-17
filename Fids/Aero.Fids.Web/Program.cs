using Aero.Fids.Web.Boards;
using Aero.ServiceDefaults;
using Aero.Shared.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var tenantConnectionString = builder.Configuration.GetConnectionString("tenant")
    ?? throw new InvalidOperationException("Connection string 'tenant' is not configured.");
var tenancyEncryptionKey = builder.Configuration["Tenancy:EncryptionKey"]
    ?? throw new InvalidOperationException("Tenancy:EncryptionKey is not configured.");

builder.Services.AddTenant(tenantConnectionString, tenancyEncryptionKey);
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddSingleton<PasswordHasherService, PasswordHasherService>();

builder.Services.AddSingleton<IFidsDbContextFactory, FidsDbContextFactory>();
builder.Services.AddScoped(sp =>
{
    var tenantId = sp.GetRequiredService<ITenantContext>().TenantId;
    var connectionString = sp.GetRequiredService<IInterfaceSettingsService>()
        .FindAsync(tenantId, InterfaceSettingKeys.FidsConnectionString).GetAwaiter().GetResult()
        ?? throw new InvalidOperationException($"No '{InterfaceSettingKeys.FidsConnectionString}' configured for tenant '{tenantId}'.");
    return sp.GetRequiredService<IFidsDbContextFactory>().Create(connectionString);
});

const string _publicBoardClientsCorsPolicy = "PublicBoardClients";
builder.Services.AddCors(options => options.AddPolicy(_publicBoardClientsCorsPolicy,
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IFlightBoardStore, FlightBoardStore>();
builder.Services.AddSingleton<IFieldMappingRegistry>(sp =>
    new FieldMappingRegistry(sp.GetRequiredService<IFidsDbContextFactory>(), sp.GetRequiredService<IInterfaceSettingsService>()));
builder.Services.AddSingleton<IStatusMappingRegistry>(sp =>
    new StatusMappingRegistry(sp.GetRequiredService<IFidsDbContextFactory>(), sp.GetRequiredService<IInterfaceSettingsService>()));
builder.Services.AddSingleton<IDisplayClientRegistry>(sp =>
    new DisplayClientRegistry(sp.GetRequiredService<IFidsDbContextFactory>(), sp.GetRequiredService<IInterfaceSettingsService>()));
builder.Services.AddSingleton<IPublicSignageReader>(sp =>
    new PublicSignageReader(sp.GetRequiredService<IFidsDbContextFactory>(), sp.GetRequiredService<IInterfaceSettingsService>()));
builder.Services.AddSingleton<IConnectedClientsRegistry, ConnectedClientsRegistry>();
builder.Services.AddHostedService<FlightUpdatesConsumer>();

builder.Services.AddScoped<IDisplayClientService, DisplayClientService>();
builder.Services.AddScoped<IFidsUserService, FidsUserService>();
builder.Services.AddScoped<IFieldMappingService, FieldMappingService>();
builder.Services.AddScoped<IStatusMappingService, StatusMappingService>();
builder.Services.AddScoped<ISignageService, SignageService>();

builder.Services.AddSingleton(new FidsImageOptions(builder.Configuration["Fids:AirlineImagesPath"]));

// https+http (not plain http) — the api project redirects http->https, and HttpClient strips the
// Authorization header when it follows a redirect, turning every authenticated call into a 401.
builder.Services.AddHttpClient<IFlightBoardBootstrapper, FlightBoardBootstrapper>(client =>
    client.BaseAddress = new Uri("https+http://api"));
builder.Services.AddHostedService<EagerBoardBootstrapper>();

builder.AddRabbitMQClient("rabbitmq");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(_publicBoardClientsCorsPolicy);

app.UseAuthentication();

// Resolves the signed-in user's tenant once per circuit/request, before any FidsDbContext use.
app.Use(async (ctx, next) =>
{
    var tenantId = ctx.User.FindFirst(FidsClaimTypes.Tenant)?.Value;
    if (!string.IsNullOrEmpty(tenantId))
    {
        var tenant = await ctx.RequestServices.GetRequiredService<ITenantStore>().FindAsync(tenantId, ctx.RequestAborted);
        if (tenant is not null)
            ctx.RequestServices.GetRequiredService<ITenantContext>().Set(tenant);
    }
    await next(ctx);
});

app.UseAuthorization();
app.UseAntiforgery();

app.MapDefaultEndpoints();
app.MapBoardEndpoints();
app.MapAuthEndpoints();
app.MapDisplayClientEndpoints();
app.MapUserEndpoints();
app.MapFieldMappingEndpoints();
app.MapStatusMappingEndpoints();
app.MapConnectedClientsEndpoints();
app.MapSignageEndpoints();
app.MapAirlineImageEndpoints();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();

---
name: add-page
description: Add a new Blazor Server page or shared component to AeroFlight.Web following the MudBlazor + cookie-auth + Api-client conventions.
---

# Add Page

Add a new page/component to `AeroFlight.Web`. Follow **all** conventions below exactly — no exceptions. This project is Blazor Server on the classic `_Host.cshtml` hosting model (not the newer Router/RCL template) — do not change the hosting model.

## Hard rule: cookie storage, not browser storage

Nothing sensitive (JWT, refresh tokens, anything secret) may ever be written to `localStorage`, `sessionStorage`, or Blazor's `ProtectedBrowserStorage`. All of it lives inside the encrypted ASP.NET Core authentication cookie via `HttpContext.SignInAsync`/claims. See "Auth setup" below.

## One-time setup (skip if already done)

### 1. MudBlazor services + theme

`Program.cs`:
```csharp
builder.Services.AddMudServices();
```

`App.razor` / `_Host.cshtml` head: MudBlazor CSS/JS references (`_content/MudBlazor/MudBlazor.min.css`, `_content/MudBlazor/MudBlazor.min.js`) plus a `<MudThemeProvider Theme="AeroFlightTheme.Default" />` near the root.

Theme file `Shared/AeroFlightTheme.cs`:
```csharp
public static class AeroFlightTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#091426",
            Success = "#10b981",
            Warning = "#f59e0b",
            Error = "#e11d48",
            Info = "#64748b",
        },
    };
}
```

### 2. Cookie authentication

`Program.cs`:
```csharp
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
```

`app.UseAuthentication(); app.UseAuthorization();` before `app.MapBlazorHub()`.

Login handler (a minimal API endpoint in `AeroFlight.Web`, not a component, since `SignInAsync` needs `HttpContext` directly):
```csharp
app.MapPost("/login", async (LoginRequest request, IApiClient api, HttpContext ctx) =>
{
    var result = await api.LoginAsync(request.Username, request.Password);
    if (result is null) return Results.Redirect("/login?error=1");

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, result.Username),
        new("api_token", result.Token),
        new("api_token_expires", result.ExpiresAt.ToString("O")),
    };
    claims.AddRange(result.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Redirect("/");
});

app.MapPost("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});
```

The `token`/`api_token_expires` claims are the *entire* mechanism for holding the JWT — the cookie carrying them is encrypted by the Data Protection API, `HttpOnly`, and never touches client-side JS or storage.

### 3. Api client

`Services/IApiClient.cs` + `Services/ApiClient.cs` — a typed `HttpClient` registered against the Aspire `api` resource:
```csharp
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri("https+http://api");
});
```
Every authenticated call pulls the JWT from the current `HttpContext.User` claims (`api_token`) via `IHttpContextAccessor` and sets `Authorization: Bearer <token>` before sending. Never persist the token anywhere else — resolve it fresh per request from the claims principal.

## Adding a page

| What | Where |
|---|---|
| Page | `src/AeroFlight.Web/Pages/<Feature>/<Name>.razor` |
| Page-specific component | `src/AeroFlight.Web/Pages/<Feature>/<Name>.<Component>.razor` |
| Shared component | `src/AeroFlight.Web/Shared/<Component>.razor` |
| Api DTOs for the page | `src/AeroFlight.Web/Services/<Feature>/*.cs` (mirror the `*Response`/`*Request` shape already returned by the matching `AeroFlight.Api` endpoint — do not redefine differently) |

Page skeleton:
```razor
@page "/counters"
@attribute [Authorize]
@inject IApiClient Api

<PageTitle>Counter Allocation</PageTitle>

<MudText Typo="Typo.h5">Counter Allocation</MudText>

@if (_counters is null)
{
    <MudProgressLinear Indeterminate="true" />
}
else
{
    <MudTable Items="_counters" Dense="true" Hover="true">
        <HeaderContent>
            <MudTh>ID</MudTh>
            <MudTh>Status</MudTh>
        </HeaderContent>
        <RowTemplate Context="counter">
            <MudTd>@counter.Code</MudTd>
            <MudTd><MudChip T="string" Color="StatusColor(counter)">@counter.Status</MudChip></MudTd>
        </RowTemplate>
    </MudTable>
}

@code {
    private IReadOnlyList<CounterResponse>? _counters;

    protected override async Task OnInitializedAsync()
        => _counters = await Api.GetCountersAsync();

    private static Color StatusColor(CounterResponse c) => c.IsActive ? Color.Success : Color.Default;
}
```

Route grouping mirrors the backend: reference-data pages live under `/reference/<plural>` matching the Api's `/api/reference/<plural>` routes.

## Status color convention (from the design mockups)

| Meaning | MudBlazor `Color` |
|---|---|
| On time / open / active / success | `Color.Success` |
| Delayed / in progress / warning | `Color.Warning` |
| Conflict / maintenance / error | `Color.Error` |
| Closed / inactive / unassigned | `Color.Default` |

## What NOT to do

- Never call `localStorage`/`sessionStorage`/`ProtectedBrowserStorage` for the JWT or any secret — the cookie via `SignInAsync` claims is the only allowed mechanism.
- Never add a direct `AppDbContext`/EF Core dependency to a page or service in this project — go through `IApiClient` (HTTP) instead, even though the csproj currently references `AeroFlight.Infrastructure`.
- Never hand-roll HTML tables/forms/buttons — use the matching MudBlazor component.
- Never build a page against an Api endpoint that doesn't exist — check `AeroFlight.Api/Endpoints/**` first; if missing, stub the UI with a disabled action and a short TODO instead of inventing backend behavior.

using Aero.Flights.Api.Endpoints.Flights;
using Aero.Flights.Api.Endpoints.Identity;
using Aero.Flights.Api.Endpoints.Metadatas;
using Aero.Flights.Api.Endpoints.Resources;
using Aero.Flights.Api.Endpoints.Rules;
using Aero.Flights.Api.Endpoints.SeasonalFlightSchedules;
using Aero.Flights.Api.Infrastructure;
using Aero.Flights.Api.Infrastructure.Authorization;
using Aero.Flights.Api.Infrastructure.Middleware;
using Aero.Flights.Application.Identity.Auth;
using Aero.Flights.Infrastructure;
using Aero.Flights.Infrastructure.Identity;
using Aero.Flights.Infrastructure.Scheduling;
using Aero.Flights.Infrastructure.Tenancy;
using Aero.ServiceDefaults;
using AeroFlight.ServiceDefaults;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Infrastructure (DB, handlers, etc.) — the AeroFlightDbContext connection is resolved per tenant.
builder.Services.AddInfrastructure();

// Quartz-scheduled jobs (in-memory job store, single shared scheduler; each job loops all tenants)
builder.Services.AddScheduledJobs();

// Tenant catalog (registry of tenant → encrypted connection string).
builder.Services.AddTenant(
    builder.Configuration.GetConnectionString("tenant")
        ?? throw new InvalidOperationException("Connection string 'tenant' is not configured."),
    builder.Configuration["Tenancy:EncryptionKey"]
        ?? throw new InvalidOperationException("Tenancy:EncryptionKey is not configured."));

// RabbitMQ (async consumers needed for the inbox consumer)
builder.AddRabbitMQClient("rabbitmq");

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<RuleValidationExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
var tenant = app.MapGroup("/{tenantId}");
tenant.RequireTenantAccessFilter();
tenant.MapAuthEndpoints();
tenant.MapUserEndpoints();
tenant.MapRoleEndpoints();
tenant.MapMetadataEndpoints();
tenant.MapAirlineContactEndpoints();
tenant.MapAirlineLogoEndpoints();
tenant.MapAirportImageEndpoints();
tenant.MapCompanyContactEndpoints();
tenant.MapRuleEndpoints();
tenant.MapFlightEndpoints();
tenant.MapFlightResourceEndpoints();
tenant.MapFlightRoutingEndpoints();
tenant.MapFlightParkPositionEndpoints();
tenant.MapFlightCodeShareEndpoints();
tenant.MapFlightDelayEndpoints();
tenant.MapFlightMovementMessageEndpoints();
tenant.MapSeasonalFlightScheduleEndpoints();
tenant.MapFlightServiceEndpoints();
tenant.MapFlightFieldEndpoints();
tenant.MapResourceEndpoints();

app.MapDefaultEndpoints();

await app.RunAsync();

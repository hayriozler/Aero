using Aero.Scanner.Domain.Enums;
using Aero.Scanner.Shared.Consts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Aero.Scanner.Api.Extensions;

public static class InstallServices
{
    public static IServiceCollection AddAeroScanJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "aeroscan",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "aeroscan-clients",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        //services.AddSingleton<IAuthorizationHandler, DebugRoleHandler>();
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.BaggageAgent, p =>
            {
                p.RequireRole(SystemDefinedRoles.BaggageAgent);
                p.RequireClaim("company_type", CompanyType.HandlingAgent.ToString());
            })
            .AddPolicy(Policies.RamSupervisor, p =>
            {
                p.RequireRole(SystemDefinedRoles.RamSupervisor);
                p.RequireClaim("company_type", CompanyType.HandlingAgent.ToString());
            })
            .AddPolicy(Policies.AirportOperator, p =>
            {
                p.RequireRole(SystemDefinedRoles.Admin);
                p.RequireClaim("company_type", CompanyType.AirportOperator.ToString());
            })
            .AddPolicy(Policies.AirportOrRamp, p => p.RequireAssertion(ctx =>
            {
                var companyType = ctx.User.FindFirst("company_type")?.Value;
                return (ctx.User.IsInRole(SystemDefinedRoles.Admin) &&
                        companyType == CompanyType.AirportOperator.ToString())
                    || (ctx.User.IsInRole(SystemDefinedRoles.RamSupervisor) &&
                        companyType == CompanyType.HandlingAgent.ToString());
            }))
            .AddPolicy(Policies.AnyRole, p => p.RequireAssertion(ctx =>
                ctx.User.IsInRole(SystemDefinedRoles.Admin) ||
                ctx.User.IsInRole(SystemDefinedRoles.RamSupervisor) ||
                ctx.User.IsInRole(SystemDefinedRoles.BaggageAgent)));

        return services;
    }
}

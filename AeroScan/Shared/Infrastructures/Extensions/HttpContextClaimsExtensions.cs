using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Extensions;

public static class HttpContextClaimsExtensions
{
    public static string GetCompanyCode(this HttpContext ctx) =>
        ctx.User.FindFirst("company_code")?.Value ?? string.Empty;    
    public static bool IsAirportOperator(this HttpContext ctx) =>
        string.Equals(ctx.User.FindFirst("company_type")?.Value,
            CompanyType.AirportOperator.ToString(), StringComparison.OrdinalIgnoreCase);

    public static bool IsHandlingAgent(this HttpContext ctx) =>
        string.Equals(ctx.User.FindFirst("company_type")?.Value,
            CompanyType.HandlingAgent.ToString(), StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns company code and whether the user is an airport operator.</summary>
    public static (string CompanyCode, bool IsAirportOperator) GetCompanyContext(this HttpContext ctx) =>
        (ctx.GetCompanyCode(), ctx.IsAirportOperator());
}

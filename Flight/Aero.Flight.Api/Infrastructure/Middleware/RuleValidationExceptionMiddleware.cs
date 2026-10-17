using Aero.Flights.Domain.Rules;

namespace Aero.Flights.Api.Infrastructure.Middleware;

public sealed class RuleValidationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (RuleValidationException ex)
        {
            await ApiResult.BadRequest(ctx, "RULE_VALIDATION_FAILED", ex.Message).ExecuteAsync(ctx);
        }
    }
}

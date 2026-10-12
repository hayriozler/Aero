using AeroFlight.Domain.Rules;

namespace AeroFlight.Api.Infrastructure.Middleware;

// Catches RuleValidationException (thrown by RuleEvaluator when a matched rule carries a
// RaiseError action) at one central point instead of every flight command handler/endpoint —
// there's no other unhandled-exception mapping in the pipeline today.
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

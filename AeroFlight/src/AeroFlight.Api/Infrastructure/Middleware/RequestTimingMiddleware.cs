namespace AeroFlight.Api.Infrastructure.Middleware;

public sealed class RequestTimingMiddleware(RequestDelegate next)
{
    internal const string StartTimeKey = "RequestStart";

    public Task InvokeAsync(HttpContext ctx)
    {
        ctx.Items[StartTimeKey] = DateTimeOffset.UtcNow;
        return next(ctx);
    }
}

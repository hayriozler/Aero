using Aero.Shared.Domain;
using Aero.Shared.Infrastructure.Diagnostics;
using Aero.Shared.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Aero.Shared.WebUtilities.Middlewares;


public sealed class SlowRequestMiddleware(RequestDelegate next, IOptions<SlowRequestOptions> options)
{
    private readonly SlowRequestOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context, QueryCollector queryCollector,
        LoggerService<SlowRequestMiddleware> logger)
    {
        var sw = Stopwatch.StartNew();

        await next(context);

        sw.Stop();

        if (sw.Elapsed <= _options.Threshold)
            return;

        var req = context.Request;
        var queries = queryCollector.Queries;

        logger.LogWarning(
            "Slow request: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms — {QueryCount} EF queries totalling {TotalQueryMs}ms",
            req.Method,
            req.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds,
            queries.Count,
            queryCollector.TotalDuration.TotalMilliseconds);

        for (var i = 0; i < queries.Count; i++)
        {
            var q = queries[i];
            logger.LogWarning(
                "  Query [{Index}/{Total}] {CommandType} ({DurationMs}ms): {Sql}",
                i + 1,
                queries.Count,
                q.CommandType,
                q.Duration.TotalMilliseconds,
                q.Sql);
        }
    }
}

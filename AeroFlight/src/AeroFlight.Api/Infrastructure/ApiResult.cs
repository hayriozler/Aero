using AeroFlight.Api.Infrastructure.Middleware;
using FluentValidation.Results;
using System.Diagnostics;

namespace AeroFlight.Api.Infrastructure;

public static class ApiResult
{
    public static IResult Ok<T>(HttpContext ctx, T data)
        => Respond(ctx, StatusCodes.Status200OK, data, null, null);

    public static IResult Created<T>(HttpContext ctx, T data, string location)
    {
        ctx.Response.Headers.Location = location;
        return Respond(ctx, StatusCodes.Status201Created, data, null, null);
    }

    public static IResult Paged<T>(HttpContext ctx, IReadOnlyList<T> data, ApiPagination pagination)
        => Respond(ctx, StatusCodes.Status200OK, data, pagination, null);

    public static IResult NoContent(HttpContext ctx)
        => Respond<object>(ctx, StatusCodes.Status204NoContent, null, null, null);

    public static IResult NotFound(HttpContext ctx, string code, string message)
        => Respond<object>(ctx, StatusCodes.Status404NotFound, null, null, [new(code, message, null)]);

    public static IResult BadRequest(HttpContext ctx, string code, string message)
        => Respond<object>(ctx, StatusCodes.Status400BadRequest, null, null, [new(code, message, null)]);

    public static IResult Conflict(HttpContext ctx, string code, string message)
        => Respond<object>(ctx, StatusCodes.Status409Conflict, null, null, [new(code, message, null)]);

    public static IResult Unauthorized(HttpContext ctx)
        => Respond<object>(ctx, StatusCodes.Status401Unauthorized, null, null,
            [new("UNAUTHORIZED", "Invalid username or password.", null)]);

    public static IResult ValidationFailed(HttpContext ctx, ValidationResult result)
    {
        var errors = result.Errors
            .Select(e => new ApiError(
                "VALIDATION_ERROR",
                e.ErrorMessage,
                string.IsNullOrEmpty(e.PropertyName) ? null : ToCamelCase(e.PropertyName)))
            .ToList();
        return Respond<object>(ctx, StatusCodes.Status422UnprocessableEntity, null, null, errors);
    }

    private static IResult Respond<T>(HttpContext ctx, int status, T? data, ApiPagination? pagination, IReadOnlyList<ApiError>? errors)
    {
        var start = ctx.Items.TryGetValue(RequestTimingMiddleware.StartTimeKey, out var s) && s is DateTimeOffset d
            ? d
            : DateTimeOffset.UtcNow;

        var traceId = Activity.Current?.TraceId.ToString() ?? ctx.TraceIdentifier;

        return Results.Json(
            new ApiResponse<T>(start, DateTimeOffset.UtcNow, status, traceId, data, pagination, errors),
            statusCode: status);
    }

    private static string ToCamelCase(string s)
        => s.Length == 0 ? s : char.ToLowerInvariant(s[0]) + s[1..];
}

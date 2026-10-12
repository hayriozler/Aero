namespace AeroFlight.Api.Infrastructure;

public record ApiResponse<T>(
    DateTimeOffset RequestStartTime,
    DateTimeOffset RequestFinishTime,
    int HttpStatus,
    string TraceId,
    T? Data,
    ApiPagination? Pagination,
    IReadOnlyList<ApiError>? Errors);

public record ApiError(string Code, string Message, string? Field);

public record ApiPagination(int Page, int PageSize, int TotalCount, int TotalPages);

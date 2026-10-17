namespace Aero.Shared.Contracts.Response;

public sealed record ApiResponse<T>(
    DateTimeOffset RequestStartTime,
    DateTimeOffset RequestFinishTime,
    int HttpStatus,
    string TraceId,
    T? Data,
    ApiPagination? Pagination,
    IReadOnlyList<ApiError>? Errors);

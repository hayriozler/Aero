namespace Aero.Shared.Contracts.Response;

public sealed record ApiPagination(int Page, int PageSize, int TotalCount, int TotalPages);

namespace Aero.Shared.Contracts.Responses;

public sealed record ApiPagination(int Page, int PageSize, int TotalCount, int TotalPages);

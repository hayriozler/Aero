namespace Aero.Shared.Contracts.Responses;

public sealed class ApiException(int statusCode, IReadOnlyList<string> errors)
    : Exception(errors.Count > 0 ? string.Join(" ", errors) : $"Request failed with status {statusCode}.")
{
    public int StatusCode { get; } = statusCode;

    public IReadOnlyList<string> Errors { get; } = errors;
}

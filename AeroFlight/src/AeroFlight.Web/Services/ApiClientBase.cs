using AeroFlight.Contracts.Identity;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AeroFlight.Web.Services;

public abstract class ApiClientBase(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected HttpClient Http => httpClient;

    // Prefixes the tenant segment (from the auth cookie) and attaches the bearer token.
    protected HttpRequestMessage CreateRequest(HttpMethod method, string uri)
    {
        var tenant = httpContextAccessor.HttpContext?.User.FindFirst(TenantClaimTypes.Tenant)?.Value;
        var prefix = string.IsNullOrEmpty(tenant) ? string.Empty : $"/{Uri.EscapeDataString(tenant)}";

        var request = new HttpRequestMessage(method, prefix + uri);
        var token = httpContextAccessor.HttpContext?.User.FindFirst("api_token")?.Value;
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return request;
    }

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        List<string> messages;
        try
        {
            var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions, ct);
            messages = envelope?.Errors?.Select(e => e.Message).ToList() ?? [];
        }
        catch
        {
            messages = [];
        }

        if (messages.Count == 0)
            messages.Add($"Request failed with status {(int)response.StatusCode}.");

        throw new ApiException((int)response.StatusCode, messages);
    }

    private async Task<ApiResponse<T>?> SendForEnvelopeAsync<T>(HttpMethod method, string uri, object? body, CancellationToken ct)
    {
        using var request = CreateRequest(method, uri);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, ct);
    }

    protected async Task<T> GetAsync<T>(string uri, CancellationToken ct)
    {
        var envelope = await SendForEnvelopeAsync<T>(HttpMethod.Get, uri, null, ct);
        return envelope!.Data!;
    }

    protected async Task<T> GetAsync<T>(string uri, T fallback, CancellationToken ct) where T : class
    {
        var envelope = await SendForEnvelopeAsync<T>(HttpMethod.Get, uri, null, ct);
        return envelope?.Data ?? fallback;
    }

    protected async Task<IReadOnlyList<T>> GetListAsync<T>(string uri, CancellationToken ct)
    {
        var envelope = await SendForEnvelopeAsync<IReadOnlyList<T>>(HttpMethod.Get, uri, null, ct);
        return envelope?.Data ?? [];
    }

    protected async Task<T> PostForResultAsync<T>(string uri, object body, CancellationToken ct)
    {
        var envelope = await SendForEnvelopeAsync<T>(HttpMethod.Post, uri, body, ct);
        return envelope!.Data!;
    }

    protected async Task<T> PutForResultAsync<T>(string uri, object body, CancellationToken ct)
    {
        var envelope = await SendForEnvelopeAsync<T>(HttpMethod.Put, uri, body, ct);
        return envelope!.Data!;
    }

    protected async Task<bool> PutForResultAsync(string uri, object body, bool fallback, CancellationToken ct)
    {
        var envelope = await SendForEnvelopeAsync<bool>(HttpMethod.Put, uri, body, ct);
        return envelope?.Data ?? fallback;
    }

    protected async Task<IReadOnlyList<T>> PutForListAsync<T>(string uri, object body, CancellationToken ct)
    {
        var envelope = await SendForEnvelopeAsync<IReadOnlyList<T>>(HttpMethod.Put, uri, body, ct);
        return envelope?.Data ?? [];
    }

    protected async Task<T?> PutForOptionalResultAsync<T>(string uri, object body, CancellationToken ct) where T : class
    {
        using var request = CreateRequest(HttpMethod.Put, uri);
        request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await Http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response, ct);
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, ct);
        return envelope?.Data;
    }

    protected async Task SendAsync(HttpMethod method, string uri, object? body, CancellationToken ct)
    {
        using var request = CreateRequest(method, uri);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await Http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);
    }

    protected async Task<bool> DeleteAsync(string uri, CancellationToken ct)
    {
        using var request = CreateRequest(HttpMethod.Delete, uri);
        using var response = await Http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        await EnsureSuccessAsync(response, ct);
        return true;
    }

    protected sealed record ApiResponse<T>(int HttpStatus, T? Data, ApiPagination? Pagination, IReadOnlyList<ApiError>? Errors);

    protected sealed record ApiError(string Code, string Message, string? Field);

    protected sealed record ApiPagination(int Page, int PageSize, int TotalCount, int TotalPages);
}

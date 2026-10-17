namespace Aero.Flights.Web.Services.Rules;

public interface IRulesClient
{
    Task<IReadOnlyList<RuleResponse>> GetAllAsync(CancellationToken ct = default);
    Task<RuleResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<RuleResponse> CreateAsync(RuleRequest request, CancellationToken ct = default);
    Task<RuleResponse> UpdateAsync(int id, RuleRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task SetEnabledAsync(int id, bool isEnabled, CancellationToken ct = default);
    Task<RuleMetadataResponse> GetMetadataAsync(CancellationToken ct = default);
}

public sealed class RulesClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IRulesClient
{
    private const string _route = "/api/rules";

    public Task<IReadOnlyList<RuleResponse>> GetAllAsync(CancellationToken ct = default)
        => GetListAsync<RuleResponse>(_route, ct);

    public Task<RuleResponse> GetByIdAsync(int id, CancellationToken ct = default)
        => GetAsync<RuleResponse>($"{_route}/{id}", ct);

    public Task<RuleResponse> CreateAsync(RuleRequest request, CancellationToken ct = default)
        => PostForResultAsync<RuleResponse>(_route, request, ct);

    public Task<RuleResponse> UpdateAsync(int id, RuleRequest request, CancellationToken ct = default)
        => PutForResultAsync<RuleResponse>($"{_route}/{id}", request, ct);

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => DeleteAsync($"{_route}/{id}", ct);

    public Task SetEnabledAsync(int id, bool isEnabled, CancellationToken ct = default)
        => SendAsync(HttpMethod.Put, $"{_route}/{id}/enabled", new RuleSetEnabledRequest(isEnabled), ct);

    public Task<RuleMetadataResponse> GetMetadataAsync(CancellationToken ct = default)
        => GetAsync<RuleMetadataResponse>($"{_route}/metadata", ct);
}

namespace Aero.Shared.Application.Managers;

public interface IAeroConfigManager
{
    Task<string?> GetAsync(string configurationName, CancellationToken ct);
    Task<int> GetIntAsync(string configurationName, int fallback, CancellationToken ct);
    Task<IReadOnlyDictionary<string, string>> GetByPrefixAsync(string prefix, CancellationToken ct);
    Task SetAsync(string configurationName, string value, CancellationToken ct);
}


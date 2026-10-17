using Aero.Flights.Application.Configuration;
using Aero.Flights.Application.Flights.Resources;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Tenancy;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Rules;

public sealed class RuleEngine(
    IAeroFlightDbContext db, FlightRuleFieldCatalog catalog, IAeroConfigService aeroConfig,
    IResourceDefaultStatusResolver statusResolver, IMemoryCache cache, ITenantContext tenant) : IRuleEngine
{
    private const string _cacheSecondsConfigName = "Rules.CacheSeconds";
    private const int _fallbackCacheSeconds = 30;

    public async Task EvaluateAsync(RuleTrigger trigger, Flight flight, CancellationToken ct)
    {
        var enabledRules = await GetEnabledRulesAsync(ct);
        var rules = enabledRules.Where(r => r.Triggers.Any(t => t.Trigger == trigger)).ToList();

        if (rules.Count == 0)
            return;

        // RuleEvaluator/FlightRuleFieldCatalog stay I/O-free by design (unit-testable without a DB),
        // so the async default-status resolution has to happen here, once, and get passed in.
        var defaultStatusCodes = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["ArrivalGate"] = await statusResolver.ResolveDefaultStatusCodeAsync<OpsArrivalGate>(ct),
            ["DepartureGate"] = await statusResolver.ResolveDefaultStatusCodeAsync<OpsDepartureGate>(ct),
            ["ArrivalBelt"] = await statusResolver.ResolveDefaultStatusCodeAsync<OpsArrivalBelt>(ct),
            ["DepartureBelt"] = await statusResolver.ResolveDefaultStatusCodeAsync<OpsDepartureBelt>(ct),
            ["Counter"] = await statusResolver.ResolveDefaultStatusCodeAsync<OpsCounter>(ct),
        };
        var resourceDefaults = new ResourceDefaults(await aeroConfig.GetByPrefixAsync("Resources.", ct), defaultStatusCodes);

        RuleEvaluator.Apply(rules, flight, catalog, resourceDefaults);
    }

    // Rules change rarely, so every enabled rule (every trigger) is cached as one entry per tenant
    // and filtered per-trigger in memory here — the rule table is small, so one cached query
    // covering every trigger beats a separate DB round trip (or cache entry) per RuleTrigger. TTL
    // is configurable via "Rules.CacheSeconds" (see AeroConfigSeeder), default 30s; set it to 0 to
    // disable caching and always read fresh. GetIntAsync is itself already cached by
    // AeroConfigService, so checking it on every call stays cheap.
    private async Task<List<Rule>> GetEnabledRulesAsync(CancellationToken ct)
    {
        var cacheSeconds = await aeroConfig.GetIntAsync(_cacheSecondsConfigName, _fallbackCacheSeconds, ct);
        if (cacheSeconds <= 0)
            return await QueryEnabledRulesAsync(ct);

        var key = RuleCacheKeys.Enabled(tenant.TenantId);
        return (await cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheSeconds);
            return await QueryEnabledRulesAsync(ct);
        }))!;
    }

    private Task<List<Rule>> QueryEnabledRulesAsync(CancellationToken ct) =>
        db.Query<Rule>()
            .AsNoTracking()
            .Where(r => r.IsEnabled)
            .Include(r => r.Triggers)
            .Include(r => r.ConditionGroups).ThenInclude(g => g.Conditions)
            .Include(r => r.Actions)
            .OrderBy(r => r.Id)
            .ToListAsync(ct);
}

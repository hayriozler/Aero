using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Rules;
using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Rules;

public sealed record GetRulesQuery;

public sealed record GetRuleQuery(int Id);

public sealed class GetRulesQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetRulesQuery, IReadOnlyList<RuleResponse>>
{
    public async Task<IReadOnlyList<RuleResponse>> HandleAsync(GetRulesQuery query, CancellationToken ct)
    {
        var rules = await db.Query<Rule>()
            .Include(r => r.Triggers)
            .Include(r => r.ConditionGroups).ThenInclude(g => g.Conditions)
            .Include(r => r.Actions)
            .OrderBy(r => r.Id)
            .ToListAsync(ct);

        return [.. rules.Select(RuleMapper.ToResponse)];
    }
}

public sealed class GetRuleQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetRuleQuery, RuleResponse?>
{
    public async Task<RuleResponse?> HandleAsync(GetRuleQuery query, CancellationToken ct)
    {
        var rule = await db.Query<Rule>()
            .Include(r => r.Triggers)
            .Include(r => r.ConditionGroups).ThenInclude(g => g.Conditions)
            .Include(r => r.Actions)
            .FirstOrDefaultAsync(r => r.Id == query.Id, ct);

        return rule is null ? null : RuleMapper.ToResponse(rule);
    }
}

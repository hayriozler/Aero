using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Tenancy;
using Aero.Flights.Contracts.Rules;
using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aero.Flights.Application.Rules;

public sealed record CreateRuleCommand(RuleRequest Request);

public sealed record UpdateRuleCommand(int Id, RuleRequest Request);

public sealed record DeleteRuleCommand(int Id);

public sealed record SetRuleEnabledCommand(int Id, bool IsEnabled);

public sealed class CreateRuleCommandHandler(IAeroFlightDbContext db, IMemoryCache cache, ITenantContext tenant)
    : ICommandHandler<CreateRuleCommand, RuleResponse>
{
    public async Task<RuleResponse> HandleAsync(CreateRuleCommand command, CancellationToken ct)
    {
        var rule = RuleMapper.FromRequest(command.Request);
        db.Add(rule);
        await db.SaveChangesAsync(ct);
        cache.Remove(RuleCacheKeys.Enabled(tenant.TenantId));
        return RuleMapper.ToResponse(rule);
    }
}

public sealed class UpdateRuleCommandHandler(IAeroFlightDbContext db, IMemoryCache cache, ITenantContext tenant)
    : ICommandHandler<UpdateRuleCommand, RuleResponse?>
{
    public async Task<RuleResponse?> HandleAsync(UpdateRuleCommand command, CancellationToken ct)
    {
        var rule = await db.Query<Rule>()
            .Include(r => r.Triggers)
            .Include(r => r.ConditionGroups).ThenInclude(g => g.Conditions)
            .Include(r => r.Actions)
            .FirstOrDefaultAsync(r => r.Id == command.Id, ct);
        if (rule is null) return null;

        RuleMapper.ApplyRequest(rule, command.Request);
        await db.SaveChangesAsync(ct);
        cache.Remove(RuleCacheKeys.Enabled(tenant.TenantId));
        return RuleMapper.ToResponse(rule);
    }
}

public sealed class DeleteRuleCommandHandler(IAeroFlightDbContext db, IMemoryCache cache, ITenantContext tenant)
    : ICommandHandler<DeleteRuleCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteRuleCommand command, CancellationToken ct)
    {
        var rule = await db.Query<Rule>().FirstOrDefaultAsync(r => r.Id == command.Id, ct);
        if (rule is null) return false;

        db.Remove(rule);
        await db.SaveChangesAsync(ct);
        cache.Remove(RuleCacheKeys.Enabled(tenant.TenantId));
        return true;
    }
}

public sealed class SetRuleEnabledCommandHandler(IAeroFlightDbContext db, IMemoryCache cache, ITenantContext tenant)
    : ICommandHandler<SetRuleEnabledCommand, bool>
{
    public async Task<bool> HandleAsync(SetRuleEnabledCommand command, CancellationToken ct)
    {
        var rule = await db.Query<Rule>().FirstOrDefaultAsync(r => r.Id == command.Id, ct);
        if (rule is null) return false;

        rule.SetEnabled(command.IsEnabled);
        await db.SaveChangesAsync(ct);
        cache.Remove(RuleCacheKeys.Enabled(tenant.TenantId));
        return true;
    }
}

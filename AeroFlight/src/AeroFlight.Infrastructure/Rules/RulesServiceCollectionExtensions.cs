using AeroFlight.Application.Common;
using AeroFlight.Application.Rules;
using AeroFlight.Contracts.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace AeroFlight.Infrastructure.Rules;

public static class RulesServiceCollectionExtensions
{
    public static IServiceCollection AddRules(this IServiceCollection services)
    {
        services.AddSingleton<FlightRuleFieldCatalog>();
        services.AddScoped<IRuleEngine, RuleEngine>();

        services.AddScoped<IQueryHandler<GetRulesQuery, IReadOnlyList<RuleResponse>>, GetRulesQueryHandler>();
        services.AddScoped<IQueryHandler<GetRuleQuery, RuleResponse?>, GetRuleQueryHandler>();
        services.AddScoped<IQueryHandler<GetRuleMetadataQuery, RuleMetadataResponse>, RuleMetadataQueryHandler>();
        services.AddScoped<ICommandHandler<CreateRuleCommand, RuleResponse>, CreateRuleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRuleCommand, RuleResponse?>, UpdateRuleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRuleCommand, bool>, DeleteRuleCommandHandler>();
        services.AddScoped<ICommandHandler<SetRuleEnabledCommand, bool>, SetRuleEnabledCommandHandler>();

        return services;
    }
}

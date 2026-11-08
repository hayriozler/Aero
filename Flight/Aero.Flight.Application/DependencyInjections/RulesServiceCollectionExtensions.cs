using Aero.Flights.Application.Rules;
using Aero.Flights.Contracts.Rules;
using Aero.Shared.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Aero.Flights.Application.DependencyInjections;

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

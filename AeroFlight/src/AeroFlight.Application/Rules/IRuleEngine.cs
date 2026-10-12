using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Rules;

namespace AeroFlight.Application.Rules;

public interface IRuleEngine
{
    Task EvaluateAsync(RuleTrigger trigger, Flight flight, CancellationToken ct);
}

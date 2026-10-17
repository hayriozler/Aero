using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Rules;

namespace Aero.Flights.Application.Rules;

public interface IRuleEngine
{
    Task EvaluateAsync(RuleTrigger trigger, Flight flight, CancellationToken ct);
}

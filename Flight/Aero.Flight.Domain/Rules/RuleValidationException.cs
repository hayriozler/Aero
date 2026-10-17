namespace Aero.Flights.Domain.Rules;

// Thrown by RuleEvaluator when a matched rule carries a RaiseError action — aborts evaluation
// (and, since it's always thrown before SaveChanges, the save itself) with the operator-authored message.
public sealed class RuleValidationException(string message) : Exception(message);

namespace Aero.Flights.Application.Common;

public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<TCommand>
{
    Task HandleAsync(TCommand command, CancellationToken ct);
}

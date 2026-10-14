namespace Aero.Scanner.Api.Services.BaggageMessagesHandlers;

public interface IBaggageMessageHandler<in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}

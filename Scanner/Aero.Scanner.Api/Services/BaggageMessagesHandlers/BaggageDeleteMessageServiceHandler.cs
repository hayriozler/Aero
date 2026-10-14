using Aero.Scanner.Api.Services.BaggageMessagesHandlers;
using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Scanner.Api.Services.BaggageMessageHandlers;

public sealed class BaggageDeleteMessageServiceHandler(LoggerService<BaggageDeleteMessageServiceHandler> logger)
    : IBaggageMessageHandler<TextMessageDepartureBagDeleteRequest>
{
    public Task HandleAsync(TextMessageDepartureBagDeleteRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}
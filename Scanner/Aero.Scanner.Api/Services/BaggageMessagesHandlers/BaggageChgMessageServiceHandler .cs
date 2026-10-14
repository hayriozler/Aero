using Aero.Scanner.Api.Persistence;
using Aero.Scanner.Api.Services.BaggageMessagesHandlers;
using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Scanner.Api.Services.BaggageMessageHandlers;

public sealed class BaggageChgMessageServiceHandler(LoggerService<BaggageChgMessageServiceHandler> logger, AeroScanDataContext dbContext) : IBaggageMessageHandler<TextMessageBumRequest>
{
    public Task HandleAsync(TextMessageBumRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

using Aero.Scanner.Api.Persistence;
using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Scanner.Api.Services.BaggageMessagesHandlers;

public sealed class BaggageBumMessageServiceHandler(LoggerService<BaggageBumMessageServiceHandler> logger, AeroScanDataContext dbContext)
    : IBaggageMessageHandler<TextMessageBumRequest>
{
    public Task HandleAsync(TextMessageBumRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}

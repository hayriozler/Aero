using Aero.Scanner.Api.Persistence;
using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Scanner.Api.Services.BaggageMessagesHandlers;

public sealed class BaggageBcmMessageServiceHandler(LoggerService<BaggageBcmMessageServiceHandler> logger, AeroScanDataContext dbContext)
    : IBaggageMessageHandler<TextMessageBcmRequest>
{
    public Task HandleAsync(TextMessageBcmRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}
using Aero.MessageProcessor.TextMessageProcessor.Persistence;
using Aero.Scanner.Shared.Dtos;
using Aero.Shared.Services;

namespace Aero.MessageProcessor.TextMessageProcessor.Services;

public sealed class BcmMessageResultHandler(LoggerService<BcmMessageResultHandler> logger)
    : IMessageResultHandler<TextMessageBcmDto>
{
    public Task HandleAsync(
        long messageId,
        TextMessageBcmDto dto,
        ITextMessageProcessorDbContext dbContext,
        CancellationToken cancellationToken)
    {
        logger.LogWarning("BCM handler for {SubType} not yet implemented", dto.SubType);
        return Task.CompletedTask;
    }
}

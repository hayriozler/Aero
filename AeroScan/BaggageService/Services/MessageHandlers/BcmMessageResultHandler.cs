using Contracts.Dtos;
using Infrastructure.Persistence;
using Infrastructure.Services;

namespace BaggageService.Services.MessageHandlers;

public sealed class BcmMessageResultHandler(LoggerService<BcmMessageResultHandler> logger)
    : IMessageResultHandler<TextMessageBcmDto>
{
    public Task HandleAsync(
        long messageId,
        TextMessageBcmDto dto,
        IDataContext dbContext,
        CancellationToken cancellationToken)
    {
        logger.LogWarning("BCM handler for {SubType} not yet implemented", dto.SubType);
        return Task.CompletedTask;
    }
}

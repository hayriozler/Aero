using Aero.MessageProcessor.TextMessageProcessor.Persistence;
using Aero.Scanner.Shared.Dtos;
using Aero.Shared.Services;

namespace Aero.MessageProcessor.TextMessageProcessor.Services;

public sealed class BumMessageResultHandler(LoggerService<BumMessageResultHandler> logger)
    : IMessageResultHandler<TextMessageBumDto>
{
    public Task HandleAsync(
        long messageId,
        TextMessageBumDto dto,
        ITextMessageProcessorDbContext dbContext,
        CancellationToken cancellationToken)
    {
        logger.LogWarning("BUM handler not yet implemented for {AirlineCode}{FlightNumber}",
            dto.AirlineCode, dto.FlightNumber);
        return Task.CompletedTask;
    }
}

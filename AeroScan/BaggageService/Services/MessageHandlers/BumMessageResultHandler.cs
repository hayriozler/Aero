using Contracts.Dtos;
using Infrastructure.Persistence;
using Infrastructure.Services;

namespace BaggageService.Services.MessageHandlers;

public sealed class BumMessageResultHandler(LoggerService<BumMessageResultHandler> logger)
    : IMessageResultHandler<TextMessageBumDto>
{
    public Task HandleAsync(
        long messageId,
        TextMessageBumDto dto,
        IDataContext dbContext,
        CancellationToken cancellationToken)
    {
        logger.LogWarning("BUM handler not yet implemented for {AirlineCode}{FlightNumber}",
            dto.AirlineCode, dto.FlightNumber);
        return Task.CompletedTask;
    }
}

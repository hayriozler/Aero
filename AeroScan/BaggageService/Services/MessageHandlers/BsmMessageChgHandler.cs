using Contracts.Dtos;
using Infrastructure.Persistence;
using Infrastructure.Services;

namespace BaggageService.Services.MessageHandlers;

public sealed class BsmMessageChgHandler(LoggerService<BsmMessageChgHandler> logger)
    : IMessageResultHandler<TextMessageDepartureBagChgDto>
{
    public Task HandleAsync(long messageId, TextMessageDepartureBagChgDto dto, IDataContext dbContext, CancellationToken cancellationToken)
    {
        Console.WriteLine(logger);
        return Task.CompletedTask;
    }
}
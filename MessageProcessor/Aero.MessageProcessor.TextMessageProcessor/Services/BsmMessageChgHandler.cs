using Aero.MessageProcessor.TextMessageProcessor.Persistence;
using Aero.Scanner.Shared.Dtos;
using Aero.Shared.Services;

namespace Aero.MessageProcessor.TextMessageProcessor.Services;

public sealed class BsmMessageChgHandler(LoggerService<BsmMessageChgHandler> logger)
    : IMessageResultHandler<TextMessageDepartureBagChgDto>
{
    public Task HandleAsync(long messageId, TextMessageDepartureBagChgDto dto, ITextMessageProcessorDbContext dbContext, CancellationToken cancellationToken)
    {
        Console.WriteLine(logger);
        return Task.CompletedTask;
    }
}
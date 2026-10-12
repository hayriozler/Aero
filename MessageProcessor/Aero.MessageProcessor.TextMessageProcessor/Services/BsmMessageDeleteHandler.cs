using Aero.MessageProcessor.TextMessageProcessor.Persistence;
using Aero.Scanner.Shared.Dtos;
using Aero.Shared.Services;

namespace Aero.MessageProcessor.TextMessageProcessor.Services;

public sealed class BsmMessageDeleteHandler(LoggerService<BsmMessageDeleteHandler> logger)
    : IMessageResultHandler<TextMessageDepartureBagDeleteDto>
{
    public Task HandleAsync(long messageId, TextMessageDepartureBagDeleteDto dto, ITextMessageProcessorDbContext dbContext, CancellationToken cancellationToken)
    {
        Console.WriteLine(logger);
        return Task.CompletedTask;
    }
}

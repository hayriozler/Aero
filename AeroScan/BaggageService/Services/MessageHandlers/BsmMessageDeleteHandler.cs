using Contracts.Dtos;
using Infrastructure.Persistence;
using Infrastructure.Services;

namespace BaggageService.Services.MessageHandlers;

public sealed class BsmMessageDeleteHandler(LoggerService<BsmMessageDeleteHandler> logger)
    : IMessageResultHandler<TextMessageDepartureBagDeleteDto>
{
    public Task HandleAsync(long messageId, TextMessageDepartureBagDeleteDto dto, IDataContext dbContext, CancellationToken cancellationToken) 
        {
        Console.WriteLine(logger);
        return Task.CompletedTask;
        }
}

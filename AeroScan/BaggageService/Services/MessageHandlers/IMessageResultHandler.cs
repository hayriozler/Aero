using Infrastructure.Persistence;

namespace BaggageService.Services.MessageHandlers;

public interface IMessageResultHandler<TDto>
{
    Task HandleAsync(long messageId, TDto dto, IDataContext dbContext, CancellationToken cancellationToken);
}

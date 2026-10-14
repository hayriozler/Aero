using Aero.TextMessageParser.Api.Persistence;

namespace Aero.TextMessageParser.Api.Services;

public interface IMessageResultHandler<TRequest>
{
    Task HandleAsync(long messageId, TRequest request, TextMessageParserDbContext dbContext, CancellationToken cancellationToken);
}

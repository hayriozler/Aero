using IataText.Parser.Entities;
using IataText.Parser.Persistence;
using Infrastructure.Persistence;

namespace IataText.Parser.Parsers;

public interface IMessageParserDispatcher
{
    Task DispatchAsync(TextMessage message, ITextParserDbContext textParserDbContext, IDataContext dataContext, CancellationToken cancellationToken);
}


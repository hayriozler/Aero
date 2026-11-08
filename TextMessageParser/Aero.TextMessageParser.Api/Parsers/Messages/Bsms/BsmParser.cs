using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Dtos;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public sealed class BsmParser(IReadOnlyDictionary<string, Element> elementMap)
    : BsmParserBase<DepartureBagDto>(null, elementMap)
{
    protected override DepartureBagDto Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToDepartureBag(messageId, elementResults, "BSM");
}

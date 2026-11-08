using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public sealed class BsmChgParser(IReadOnlyDictionary<string, Element> elementMap)
    : BsmParserBase<DepartureBagDto>(Consts.CHG, elementMap)
{
    protected override DepartureBagDto Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToDepartureBag(messageId, elementResults, "BSM CHG");
}

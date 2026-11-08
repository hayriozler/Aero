using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public sealed class BsmDelParser(IReadOnlyDictionary<string, Element> elementMap)
    : BsmParserBase<BagRemovalDto>(Consts.DEL, elementMap)
{
    protected override BagRemovalDto Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToBagRemoval(messageId, elementResults, "BSM DEL");
}

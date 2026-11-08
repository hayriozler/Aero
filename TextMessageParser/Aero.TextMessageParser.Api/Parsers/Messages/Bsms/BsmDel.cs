using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public sealed class BsmDel(IReadOnlyDictionary<string, Element> elementMap)
    : BsmBase<TextMessageDepartureBagDeleteDto>(Consts.DEL, elementMap)
{
    protected override TextMessageDepartureBagDeleteDto Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToDepartureBagDelete(messageId, elementResults);
}

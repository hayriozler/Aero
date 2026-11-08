using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public sealed class BsmChg(IReadOnlyDictionary<string, Element> elementMap)
    : BsmBase<TextMessageDepartureBagChgDto>(Consts.CHG, elementMap)
{
    protected override TextMessageDepartureBagChgDto Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToDepartureBagChange(messageId, elementResults);
}

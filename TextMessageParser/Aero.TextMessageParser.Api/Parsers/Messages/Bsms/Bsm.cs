using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Requests;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public sealed class Bsm(IReadOnlyDictionary<string, Element> elementMap)
    : BsmBase<TextMessageDepartureBagRequest>(null, elementMap)
{
    protected override TextMessageDepartureBagRequest Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToDepartureBag(messageId, elementResults);
}

using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bcms;

public sealed class BcmFom(IReadOnlyDictionary<string, Element> elementMap)
    : BcmBase(Consts.FOM, elementMap);

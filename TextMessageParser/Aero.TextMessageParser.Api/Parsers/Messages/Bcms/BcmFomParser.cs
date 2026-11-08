using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bcms;

public sealed class BcmFomParser(IReadOnlyDictionary<string, Element> elementMap)
    : BcmParserBase(Consts.FOM, elementMap);

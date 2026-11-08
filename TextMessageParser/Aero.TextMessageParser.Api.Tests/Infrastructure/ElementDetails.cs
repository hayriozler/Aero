using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class ElementDetails
{
    public static ElementDetail Make(string line, ElementRequirement requirement)
    {
        var mem = line.AsMemory();
        return new ElementDetail(mem[..Math.Min(3, mem.Length)], mem, ParserExtensions.SplitOnSlash(mem), 2 + mem.Length, requirement);
    }
}

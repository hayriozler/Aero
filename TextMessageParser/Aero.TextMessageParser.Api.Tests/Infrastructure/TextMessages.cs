using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Extensions;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class TextMessages
{
    public static TextMessage From(string raw, long id = 1)
    {
        var (header, footer) = raw.GetMessageIdentifier();
        var message = TextMessage.Create(raw, header, footer);
        message.Id = id;
        return message;
    }
}

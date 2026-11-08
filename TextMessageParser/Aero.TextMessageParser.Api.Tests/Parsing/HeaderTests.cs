using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Aero.TextMessageParser.Api.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class HeaderTests
{
    private static IReadOnlyDictionary<string, IMessageHandler> Handlers()
    {
        var services = new ServiceCollection();
        services.InstallElements().InstallMessages();
        return services.BuildServiceProvider().GetRequiredService<IReadOnlyDictionary<string, IMessageHandler>>();
    }

    [Theory(DisplayName = "Header: BSM CHG and BSM DEL keep CHG/DEL as the change of status indicator, BCM keeps FOM/FCM/BAM/DBM as the secondary level identifier (IATA table 2.2)")]
    [InlineData("BSM", null, null, null)]
    [InlineData("BSM", "CHG", null, "CHG")]
    [InlineData("BSM", "DEL", null, "DEL")]
    [InlineData("BCM", "FOM", "FOM", null)]
    [InlineData("BCM", "DBM", "DBM", null)]
    public void Second_line_goes_to_the_right_header_field(string type, string? second, string? expectedSecondary, string? expectedChange)
    {
        var raw = Build(type, second, NativeNewLine, V, F);
        var (header, _) = raw.GetMessageIdentifier();
        Check.Equal(type, header.Identifier, "Identifier.");
        Check.True(header.SecondaryIdentifier == expectedSecondary, $"Secondary identifier was '{header.SecondaryIdentifier}', expected '{expectedSecondary}'.");
        Check.True(header.ChangeOfStatus == expectedChange, $"Change of status was '{header.ChangeOfStatus}', expected '{expectedChange}'.");
    }

    [Fact(DisplayName = "Header: the routing key of every registered handler equals the key it is registered under")]
    public void Registered_keys_match_the_handlers_own_header()
    {
        var handlers = Handlers();
        Check.True(handlers.Count > 0, "Handlers must be registered.");
        foreach (var (key, handler) in handlers)
            Check.Equal(key, handler.RoutingKey, $"Handler {handler.MessageName} is registered under '{key}' but its own header says '{handler.RoutingKey}'.");
    }

    [Theory(DisplayName = "Header: the routing key read from stored message text finds the matching handler for every supported message type")]
    [InlineData("BSM", null)]
    [InlineData("BSM", "CHG")]
    [InlineData("BSM", "DEL")]
    [InlineData("BUM", null)]
    [InlineData("BCM", "FOM")]
    [InlineData("BCM", "FCM")]
    [InlineData("BCM", "BAM")]
    [InlineData("BCM", "DBM")]
    public void Parsed_header_finds_its_handler(string type, string? second)
    {
        var raw = Build(type, second, NativeNewLine, V, F);
        var (header, _) = raw.GetMessageIdentifier();
        Check.True(Handlers().ContainsKey(header.RoutingKey), $"No handler is registered for routing key '{header.RoutingKey}' ({type} {second}).");
    }

    [Fact(DisplayName = "Header: a message stored the old way (CHG in the secondary identifier column) still routes to the same handler and skips its CHG line")]
    public void Old_rows_still_route_and_parse()
    {
        var raw = ChgText(V, F, N1, P);
        var (parsed, footer) = raw.GetMessageIdentifier();
        var oldStyle = TextMessage.Create(raw, new MessageHeader("BSM", "CHG", null), footer);
        oldStyle.Id = 1;
        Check.Equal(parsed.RoutingKey, oldStyle.Header.RoutingKey, "Old and new storage must give the same routing key.");
        var handler = new Aero.TextMessageParser.Api.Parsers.Messages.Bsms.BsmChg(ElementMapFactory.Create());
        var result = handler.Parse(oldStyle);
        Check.True(result.IsSuccess, "A CHG row that was stored with CHG in the secondary identifier column must still parse.");
    }
}

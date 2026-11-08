using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Processing;

public class ParsedMessageTests
{
    [Fact(DisplayName = "Bug 14: a BSM with .V .F .N .P is mapped to a parsed message with its bag tag, flight and passenger name")]
    [Trait("Bug", "14")]
    public void Parsed_message_carries_the_parsed_values()
    {
        var message = TextMessages.From(BsmText(V, F, N1, P));
        var result = new BsmParser(ElementMapFactory.Create()).Parse(message).Value;
        var parsed = ParsedMessageFactory.Create(message, result!);

        Check.True(parsed is not null, "A BSM result must be mapped.");
        Check.Equal("LH", parsed!.AirlineCode, "Airline code.");
        Check.Equal("400", parsed.FlightNumber, "Flight number.");
        Check.SequenceEqual(new[] { "0220123456" }, parsed.BagTags.Select(t => t.BagTag), "Bag tags.");
        Check.Equal("JOHN DOE", parsed.PassengerName, "Passenger name.");
        Check.True(!parsed.IsConsumed, "A new parsed message is not consumed.");
    }

    [Fact(DisplayName = "Bug 14: a BSM without .S and .P must not store the default char or empty strings (PostgreSQL rejects a NUL character)")]
    [Trait("Bug", "14")]
    public void Missing_passenger_elements_become_null()
    {
        var message = TextMessages.From(BsmText(V, F, N1));
        var result = new BsmParser(ElementMapFactory.Create()).Parse(message).Value;
        var parsed = ParsedMessageFactory.Create(message, result!)!;

        Check.True(parsed.PassengerStatus is null, "Status must be null, not '\\0'.");
        Check.True(parsed.PassengerSequenceNumber is null, "Sequence number must be null, not an empty string.");
        Check.True(parsed.SeatNumber is null, "Seat must be null.");
    }

    [Fact(DisplayName = "Bug 14: MarkConsumed sets the flag and the time")]
    [Trait("Bug", "14")]
    public void Mark_consumed_sets_flag()
    {
        var message = TextMessages.From(BsmText(V, F, N1));
        var result = new BsmParser(ElementMapFactory.Create()).Parse(message).Value;
        var parsed = ParsedMessageFactory.Create(message, result!)!;
        var at = new DateTime(2026, 11, 8, 12, 0, 0, DateTimeKind.Utc);

        parsed.MarkConsumed(at);

        Check.True(parsed.IsConsumed, "Consumed flag.");
        Check.True(parsed.ConsumedAt == at, "Consumed time.");
    }
}

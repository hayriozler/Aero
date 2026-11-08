using Aero.TextMessageParser.Api.Services;
using System.Net;
using System.Net.Http.Json;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class EnvelopeTests
{
    [Theory(DisplayName = "Bug 12: GetMessageIdentifier must not throw ArgumentOutOfRangeException for null, empty, one-line, whitespace-only or blank-line-only messages")]
    [Trait("Bug", "12")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("BSM")]
    [InlineData("         ")]
    [InlineData("\r\n\r\n\r\n")]
    public void GetMessageIdentifier_degenerate_input_does_not_throw_index_exceptions(string? text)
    {
        const string Why = "GetMessageIdentifier indexes splittedText[0] and splittedText[1] without checking the number of non-empty lines, so degenerate input escapes as ArgumentOutOfRangeException (and the endpoint turns it into HTTP 500 instead of 400). Expected: a controlled result or a deliberate validation exception, never an index exception.";
        var ex = Record.Exception(() => text!.GetMessageIdentifier());
        Check.True(ex is null || ex is not (ArgumentOutOfRangeException or IndexOutOfRangeException),
            $"Input <{text ?? "null"}> threw {ex?.GetType().Name}. {Why}");
    }

    [Fact(DisplayName = "Bug 12: degenerate text is reported through TryGetMessageIdentifier returning false and GetMessageIdentifier throwing FormatException")]
    [Trait("Bug", "12")]
    public void Degenerate_text_is_rejected_in_a_controlled_way()
    {
        Check.True(!"BSM".TryGetMessageIdentifier(out _, out _), "A one-line text has no elements and no footer, TryGetMessageIdentifier must return false.");
        var ex = Record.Exception(() => "BSM".GetMessageIdentifier());
        Check.True(ex is FormatException, $"GetMessageIdentifier must throw FormatException for invalid text; observed {ex?.GetType().Name ?? "no exception"}.");
    }

    [Fact(DisplayName = "Bug 12: a BSM whose last line is END of a different message type (ENDBUM) must be rejected by the BSM handler")]
    [Trait("Bug", "12")]
    public void Bsm_with_mismatching_end_line_is_rejected()
    {
        const string Why = "The footer is taken blindly from the last line of the text, then skipped by SplitSpanMessage, so a BSM terminated with ENDBUM is processed as if valid. Expected: null result plus a recorded message error.";
        var raw = string.Join(SampleMessages.NativeNewLine, "BSM", SampleMessages.V, SampleMessages.F, SampleMessages.N1, SampleMessages.P, "ENDBUM");
        var bsm = new Bsm(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(raw), db);
        Check.True(dto is null && db.TextMessageErrorSet.Local.Count > 0, Why);
    }

    [Fact(DisplayName = "Bug 12: a BSM with no END line at all must be rejected, not silently lose its last element")]
    [Trait("Bug", "12")]
    public void Bsm_without_end_line_is_rejected()
    {
        const string Why = "Without END, the last real element (.P/DOE/JOHN) is taken as the footer and silently dropped; the BSM is accepted. Expected: null result plus a recorded message error.";
        var raw = string.Join(SampleMessages.NativeNewLine, "BSM", SampleMessages.V, SampleMessages.F, SampleMessages.N1, SampleMessages.P);
        var bsm = new Bsm(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(raw), db);
        Check.True(dto is null && db.TextMessageErrorSet.Local.Count > 0, Why);
    }

    [Theory(DisplayName = "Bug 12 (endpoint): POST /api/textmessages answers 400 for null, missing, whitespace-only and one-line messages")]
    [Trait("Bug", "12")]
    [InlineData("{\"message\":null}")]
    [InlineData("{}")]
    [InlineData("{\"message\":\"         \"}")]
    [InlineData("{\"message\":\"BSM-------\"}")]
    public async Task Endpoint_returns_bad_request_for_degenerate_bodies(string json)
    {
        const string Why = "A null message dereferences request.Message.Length and a 9+ character one-liner reaches GetMessageIdentifier; both end in the exception middleware as HTTP 500. Expected: HTTP 400.";
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/textmessages", content);
        Check.Equal(HttpStatusCode.BadRequest, response.StatusCode, $"Body {json}. {Why}");
    }

    [Theory(DisplayName = "Bug 12 (endpoint): POST /api/textmessages answers 400 for a BSM with mismatching or missing END line")]
    [Trait("Bug", "12")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Endpoint_rejects_bad_footer(bool mismatching)
    {
        const string Why = "The endpoint stores any text of 9+ characters whose first lines look like a header. Expected: HTTP 400 when the last line is not END followed by the message type.";
        var lines = new List<string> { "BSM", SampleMessages.V, SampleMessages.F, SampleMessages.N1, SampleMessages.P };
        if (mismatching) lines.Add("ENDBUM");
        var raw = string.Join(SampleMessages.NativeNewLine, lines);
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = raw });
        Check.Equal(HttpStatusCode.BadRequest, response.StatusCode, Why);
    }

    [Fact(DisplayName = "Baseline: endpoint stores a valid BSM and answers 202 Accepted")]
    [Trait("Kind", "Baseline")]
    public async Task Endpoint_accepts_valid_bsm()
    {
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = SampleMessages.SingleTagBsm() });
        Check.Equal(HttpStatusCode.Accepted, response.StatusCode, "A well-formed BSM must be accepted for later processing.");
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
        Check.Equal(1, db.TextMessagesSet.Count(), "Exactly one TextMessage row must be stored.");
    }

    [Fact(DisplayName = "Baseline: GetMessageIdentifier reads BSM header, CHG secondary identifier and ENDBSM footer")]
    [Trait("Kind", "Baseline")]
    public void GetMessageIdentifier_reads_header_and_footer()
    {
        var (plain, plainFooter) = SampleMessages.SingleTagBsm().GetMessageIdentifier();
        Check.Equal("BSM", plain.Identifier, "Header identifier.");
        Check.True(plain.SecondaryIdentifier is null, "Plain BSM has no secondary identifier.");
        Check.Equal("ENDBSM", plainFooter.EndIdentifier, "Footer identifier.");
        var (chg, _) = SampleMessages.ChgText(SampleMessages.V, SampleMessages.N1).GetMessageIdentifier();
        Check.Equal("CHG", chg.ChangeOfStatus, "CHG is the change of status indicator (IATA table 2.2).");
        Check.True(chg.SecondaryIdentifier is null, "CHG is not a secondary level identifier.");
        var (del, _) = SampleMessages.DelText(SampleMessages.V, SampleMessages.N1).GetMessageIdentifier();
        Check.Equal("DEL", del.ChangeOfStatus, "DEL is the change of status indicator (IATA table 2.2).");
        Check.True(del.SecondaryIdentifier is null, "DEL is not a secondary level identifier.");
    }

    [Theory(DisplayName = "Bug 13: the same BSM using LF only and CRLF must give identical header/footer from GetMessageIdentifier regardless of Environment.NewLine")]
    [Trait("Bug", "13")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void GetMessageIdentifier_is_independent_of_line_ending(string newLine)
    {
        const string Why = "Consts.NEW_LINE_SEPERATOR depends on Environment.NewLine and Span.Split(char[]) splits on the whole sequence. On Windows an LF-only message is not split at all; on Linux CRLF messages keep a trailing CR on each line. Expected: both line endings parse the same everywhere.";
        var raw = SampleMessages.Build("BSM", null, newLine, SampleMessages.V, SampleMessages.F, SampleMessages.N1, SampleMessages.P);
        var ex = Record.Exception(() =>
        {
            var (h, f) = raw.GetMessageIdentifier();
            Check.Equal("BSM", h.Identifier, Why);
            Check.Equal("ENDBSM", f.EndIdentifier, Why);
        });
        Check.True(ex is null, $"Line ending {(newLine == "\n" ? "LF" : "CRLF")} threw {ex?.GetType().Name}: {ex?.Message}. {Why}");
    }

    [Theory(DisplayName = "Bug 13: the same BSM using LF only and CRLF must produce the same DTO from Bsm.ProcessMessage")]
    [Trait("Bug", "13")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void ProcessMessage_is_independent_of_line_ending(string newLine)
    {
        const string Why = "Same message, different line ending, must give the same DTO (one tag 0220123456, passenger JOHN DOE).";
        var raw = SampleMessages.Build("BSM", null, newLine, SampleMessages.V, SampleMessages.F, SampleMessages.N1, SampleMessages.P);
        var bsm = new Bsm(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        Requests.TextMessageDepartureBagRequest? dto = null;
        var ex = Record.Exception(() => dto = bsm.ProcessMessage(TextMessages.From(raw), db));
        Check.True(ex is null, $"Threw {ex?.GetType().Name}: {ex?.Message}. {Why}");
        Check.True(dto is not null, "Result was null. " + Why);
        Check.SequenceEqual(new[] { "0220123456" }, dto!.BagTags, Why);
        Check.Equal("JOHN DOE", dto.PassengerName, Why);
    }

    [Fact(DisplayName = "Bug 13 (low priority): Consts.NEW_LINE_SEPERATOR must not allocate a new array on every access")]
    [Trait("Bug", "13")]
    [Trait("Priority", "Low")]
    public void NewLineSeparator_is_cached()
    {
        Check.True(ReferenceEquals(Consts.NEW_LINE_SEPERATOR, Consts.NEW_LINE_SEPERATOR),
            "The property is an expression-bodied getter that builds a new char[] on every call, which the parser does per message and per line.");
    }
}

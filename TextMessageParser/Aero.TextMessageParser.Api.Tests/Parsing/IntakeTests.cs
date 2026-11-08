using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class IntakeTests
{
    [Fact(DisplayName = "Endpoint: a valid message is answered with 202 Accepted and the id of the stored message")]
    public async Task Accepted_response_carries_the_stored_id()
    {
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = SingleTagBsm() });
        Check.Equal(HttpStatusCode.Accepted, response.StatusCode, "The message is stored for later processing, not processed yet.");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        using var scope = factory.Services.CreateScope();
        var stored = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>().TextMessagesSet.Single();
        Check.True(body.GetProperty("id").GetInt64() == stored.Id, "The response must carry the id of the stored row so the sender can refer to it.");
    }

    [Fact(DisplayName = "Endpoint: a message longer than the limit is answered with 413 and nothing is stored")]
    public async Task Too_long_message_is_rejected()
    {
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        var raw = "BSM\n.V/7LFRA\n.R/" + new string('X', Consts.MaxMessageLength) + "\nENDBSM";
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = raw });
        Check.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode, "Message longer than the limit.");
        using var scope = factory.Services.CreateScope();
        Check.True(!scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>().TextMessagesSet.Any(), "A rejected message must not be stored.");
    }

    [Fact(DisplayName = "Endpoint: a message of exactly the maximum length is accepted")]
    public async Task Message_at_the_limit_is_accepted()
    {
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        const string Head = "BSM\n.V/7LFRA\n.R/";
        const string Tail = "\nENDBSM";
        var raw = Head + new string('X', Consts.MaxMessageLength - Head.Length - Tail.Length) + Tail;
        Check.Equal(Consts.MaxMessageLength, raw.Length, "Test message must be exactly at the limit.");
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = raw });
        Check.Equal(HttpStatusCode.Accepted, response.StatusCode, "The limit itself is allowed.");
    }

    [Fact(DisplayName = "Endpoint: a rejected message is answered with a problem details body")]
    public async Task Bad_request_is_a_problem_details_response()
    {
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = "BSM" });
        Check.Equal(HttpStatusCode.BadRequest, response.StatusCode, "One line message.");
        Check.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType, "Rejections use the standard problem details format.");
    }

    [Theory(DisplayName = "Spaces: a header line or an END line with a space around it is rejected at the endpoint")]
    [InlineData("BSM \n.V/7LFRA\n.F/LH400/12NOV/JFK/Y\n.N/0220123456001\nENDBSM")]
    [InlineData("BSM\n.V/7LFRA\n.F/LH400/12NOV/JFK/Y\n.N/0220123456001\nENDBSM ")]
    [InlineData(" BSM\n.V/7LFRA\n.F/LH400/12NOV/JFK/Y\n.N/0220123456001\nENDBSM")]
    public async Task Padded_envelope_lines_are_rejected(string raw)
    {
        using var factory = new ApiFactory(removeProcessingService: true);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/textmessages", new { message = raw });
        Check.Equal(HttpStatusCode.BadRequest, response.StatusCode, "Spaces are not allowed around header and END lines.");
    }

    [Theory(DisplayName = "Spaces: padded CHG, END and element lines make the parser reject the message")]
    [InlineData("BSM\nCHG \n.V/7LFRA\n.F/LH400/12NOV/JFK/Y\n.N/0220123456001\nENDBSM")]
    [InlineData("BSM\nCHG\n.V/7LFRA\n.F/LH400/12NOV/JFK/Y\n.N/0220123456001\nENDBSM ")]
    [InlineData("BSM\nCHG\n.V/7LFRA\n.F/LH400/12NOV/JFK/Y\n.N/0220123456001 \nENDBSM")]
    public void Padded_lines_are_rejected_by_the_parser(string raw)
    {
        var result = new BsmChg(ElementMapFactory.Create()).Parse(TextMessages.From(raw));
        Check.True(!result.IsSuccess, "A line with a trailing space must not be accepted silently.");
    }
}

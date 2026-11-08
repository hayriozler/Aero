using System.Text.Json;
using Aero.TextMessageParser.Api.Parsers.Messages.Bcms;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class ParseResultTests
{
    [Fact(DisplayName = "Parse: a valid BSM yields a value and no errors, without any database involved")]
    [Trait("Kind", "Baseline")]
    public void Valid_message_succeeds_without_a_database()
    {
        var result = new Bsm(ElementMapFactory.Create()).Parse(TextMessages.From(SingleTagBsm()));
        Check.True(result.IsSuccess, "A well-formed BSM must parse.");
        Check.True(result.Errors.Count == 0, "A successful parse has no errors.");
        Check.SequenceEqual(new[] { "0220123456" }, result.Value!.BagTags, "The bag tag of the single .N/ line.");
    }

    [Fact(DisplayName = "Parse: a message level problem (wrong END line) is reported as a message error with the END error code")]
    public void Message_level_problem_is_a_message_error()
    {
        var raw = Build("BSM", null, NativeNewLine, V, F, N1, P).Replace("ENDBSM", "ENDBUM");
        var result = new Bsm(ElementMapFactory.Create()).Parse(TextMessages.From(raw));
        Check.True(!result.IsSuccess && result.Value is null, "A message with a wrong END line must not produce a value.");
        Check.True(result.Errors.Count == 1 && !result.Errors[0].IsElementError, "The problem belongs to the message, not to an element.");
        Check.Equal("MSG_BSM_END_INVALID", result.Errors[0].ErrorCode, "Error code.");
    }

    [Fact(DisplayName = "Parse: an element level problem (malformed .F/) is reported as an element error")]
    public void Element_level_problem_is_an_element_error()
    {
        var result = new Bsm(ElementMapFactory.Create()).Parse(TextMessages.From(BsmText(V, ".F/LH400", N1, P)));
        Check.True(!result.IsSuccess, "A malformed .F/ line must not produce a value.");
        Check.True(result.Errors.Count == 1 && result.Errors[0].IsElementError, "The problem belongs to an element.");
    }

    [Fact(DisplayName = "Mapping: BSM and BSM CHG with the same elements produce the same data in their DTOs (the mapping code is shared)")]
    public void Bsm_and_chg_share_the_same_mapping()
    {
        const string S = ".S/Y/12A/B/001/123/Y/N/A";
        const string W = ".W/K/001/20";
        var map = ElementMapFactory.Create();
        var create = new Bsm(map).Parse(TextMessages.From(BsmText(V, F, N1, N2, ".O/EW9021/14NOV/BER/C", S, W, P))).Value;
        var change = new BsmChg(map).Parse(TextMessages.From(ChgText(V, F, N1, N2, ".O/EW9021/14NOV/BER/C", S, W, P))).Value;
        Check.True(create is not null && change is not null, "Both messages must parse.");
        Check.Equal(JsonSerializer.Serialize(create), JsonSerializer.Serialize(change), "BSM and BSM CHG DTOs must carry identical data for identical elements.");
        Check.SequenceEqual(new[] { "0220123456", "0220123457" }, create!.BagTags, "Both bag tags.");
        Check.Equal("JOHN DOE", create.PassengerName, "Passenger name.");
        Check.True(create.Onwards.Count == 1 && create.Onwards[0].Destination == "BER", "Onward flight.");
    }

    [Fact(DisplayName = "Mapping: BSM DEL reads the flight, destination and bag tags from the shared field reader")]
    public void Delete_uses_the_shared_reader()
    {
        var dto = new BsmDel(ElementMapFactory.Create()).Parse(TextMessages.From(DelText(V, F, N1, N2))).Value;
        Check.True(dto is not null, "BSM DEL must parse.");
        Check.SequenceEqual(new[] { "0220123456", "0220123457" }, dto!.BagTags, "Bag tags.");
        Check.Equal("LH", dto.airlineCode, "Airline code.");
        Check.Equal("JFK", dto.destination, "Destination.");
    }

    [Theory(DisplayName = "Mapping: BCM FOM, FCM and DBM report their own sub type and the flight of the .F/ element through one shared handler")]
    [Trait("Kind", "Baseline")]
    [InlineData("FOM", "Fom")]
    [InlineData("FCM", "Fcm")]
    [InlineData("DBM", "Dbm")]
    public void Bcm_variants_share_the_handler(string identifier, string expectedSubType)
    {
        var map = ElementMapFactory.Create();
        Aero.TextMessageParser.Api.Parsers.Messages.MessageBase<Aero.TextMessageParser.Api.Dtos.TextMessageBcmDto> handler = identifier switch
        {
            "FOM" => new BcmFom(map),
            "FCM" => new BcmFcm(map),
            _ => new BcmDbm(map)
        };
        var dto = handler.Parse(TextMessages.From(BcmText(identifier, V, ".F/LH400/12NOV/JFK"))).Value;
        Check.True(dto is not null, "BCM must parse.");
        Check.Equal(expectedSubType, dto!.SubType, "Sub type.");
        Check.Equal("LH", dto.AirlineCode, "Airline code.");
        Check.Equal("400", dto.FlightNumber, "Flight number.");
    }

    [Fact(DisplayName = "Mapping: a BSM without .V/ has no source indicator and the mapper reports which message type is incomplete")]
    public void Missing_source_indicator_names_the_message_type()
    {
        var parser = new BsmChg(ElementMapFactory.Create());
        var result = parser.Parse(TextMessages.From(ChgText(F, N1, P)));
        Check.True(!result.IsSuccess, "A BSM CHG without the mandatory .V/ element must not produce a value.");
    }
}

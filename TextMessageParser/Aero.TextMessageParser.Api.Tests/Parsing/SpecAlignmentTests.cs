using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Elements.Validators;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Parsers.Messages.Bcms;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class SpecAlignmentTests
{
    private static bool IsValid(IElementValidator validator, string line)
        => validator.Validate(ElementDetails.Make(line, ElementRequirement.Optional)).IsValid;

    [Theory(DisplayName = "Spec 2.3.27: .X accepts empty fields and the REJ result (examples from the IATA 1745 text)")]
    [Trait("Kind", "Spec")]
    [InlineData(".X/SEL")]
    [InlineData(".X/NON")]
    [InlineData(".X//CLR//AT/TSA123")]
    [InlineData(".X//REJ/D/XRAY/BUZZC007/Colonel Mustard in the cellar")]
    public void X_accepts_spec_examples(string line)
        => Check.True(IsValid(new ElementXValidator(), line), $"'{line}' is an example of the standard and must be accepted.");

    [Fact(DisplayName = "Spec 2.3.27: .X rejects the misspelled result REF")]
    [Trait("Kind", "Spec")]
    public void X_rejects_ref()
        => Check.True(!IsValid(new ElementXValidator(), ".X/SEL/REF"), "REF is not a result indicator, REJ is.");

    [Theory(DisplayName = "Spec 2.3.24: .U accepts empty fields (examples from the IATA 1745 text)")]
    [Trait("Kind", "Spec")]
    [InlineData(".U//AFTRT/X/F/DEN")]
    [InlineData(".U///T//MIA//AA614")]
    [InlineData(".U/CAR1357KL//M")]
    [InlineData(".U/AVE4321SQ/024L/T/Y/HKG")]
    public void U_accepts_spec_examples(string line)
        => Check.True(IsValid(new ElementUValidator(), line), $"'{line}' is an example of the standard and must be accepted.");

    [Theory(DisplayName = "Spec 2.3.11: .H accepts the short forms of the standard, including .H///D46")]
    [Trait("Kind", "Spec")]
    [InlineData(".H/T4//16")]
    [InlineData(".H//GREEN")]
    [InlineData(".H///D46")]
    [InlineData(".H///CLAIMA5")]
    public void H_accepts_spec_examples(string line)
        => Check.True(IsValid(new ElementHValidator(), line), $"'{line}' is an example of the standard and must be accepted.");

    [Theory(DisplayName = "Spec 2.3.8 and 2.3.20: .E needs 3 or 4 letters, .Q needs exactly 3 digits")]
    [Trait("Kind", "Spec")]
    [InlineData("E", ".E/RUSH", true)]
    [InlineData("E", ".E/MIS", true)]
    [InlineData("E", ".E/R1", false)]
    [InlineData("E", ".E/TOOLONG", false)]
    [InlineData("Q", ".Q/006", true)]
    [InlineData("Q", ".Q/6", false)]
    [InlineData("Q", ".Q/0061", false)]
    [InlineData("Q", ".Q/00A", false)]
    public void E_and_Q_follow_the_standard(string element, string line, bool expected)
    {
        IElementValidator validator = element == "E" ? new ElementEValidator() : new ElementQValidator();
        Check.True(IsValid(validator, line) == expected, $"'{line}' expected valid={expected}.");
    }

    [Fact(DisplayName = "Spec 4.6: a BSM may repeat .E, .R and .G")]
    [Trait("Kind", "Spec")]
    public void Bsm_repeats_e_r_g()
    {
        using var db = TestDbContextFactory.Create();
        var text = BsmText(V, F, N1, ".G/25NOV1600/25NOV2200/Willem de Zwijger laan 234", ".G/Zuid Holland/Nederland",
            ".E/IROP", ".E/UNAC", ".E/RRTE", ".E/STCK", ".R/VIP", ".R/DELAYED INBOUND");
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(text), db);
        Check.True(dto is not null, "The message follows the standard and must parse.");
        Check.Equal(0, db.ElementErrors.Local.Count + db.MessageErrors.Local.Count, "No error rows.");
    }

    [Fact(DisplayName = "Spec 4.5: .A is not an element of the BSM")]
    [Trait("Kind", "Spec")]
    public void Bsm_does_not_accept_a()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(
            TextMessages.From(BsmText(V, F, N1, ".A/QF006201/BSM/ACK")), db);
        Check.True(dto is null, "A BSM with .A must not parse.");
        Check.True(db.MessageErrors.Local.Any(e => e.ErrorCode == ErrorCodes.UNKNOWN_ELEMENT_FOUND), "UNKNOWN_ELEMENT_FOUND is recorded.");
    }

    [Fact(DisplayName = "Spec 6.5: a BUM with two bags repeats .U, .N, .Q, .P (example 6.9.2)")]
    [Trait("Kind", "Spec")]
    public void Bum_with_two_bags_parses()
    {
        using var db = TestDbContextFactory.Create();
        var text = Build("BUM", null, NativeNewLine,
            ".V/1LPIT////123ABC456Z", ".F/US816/01MAY/FRA/Y",
            ".U/AVE0321US/0012/T/Y/FRA", ".N/0037336127001", ".Q/026", ".P/WILSON/SARA",
            ".U/AKV9876US//T/Y/FRA", ".N/0125886172001", ".Q/012", ".P/BROWN/LEE");
        var dto = new BumParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(text), db);
        Check.True(dto is not null, "The message follows the standard and must parse. " + string.Join("; ", db.MessageErrors.Local.Select(e => e.ErrorCode + " " + e.ErrorDescription).Concat(db.ElementErrors.Local.Select(e => e.ErrorCode + " " + e.ErrorDescription))));
        Check.SequenceEqual(new[] { "0037336127", "0125886172" }, dto!.BagTags, "Both bag tags.");
    }

    [Fact(DisplayName = "Spec 6.5: a BUM without .F is rejected because .F is mandatory")]
    [Trait("Kind", "Spec")]
    public void Bum_requires_f()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BumParser(ElementMapFactory.Create()).ProcessMessage(
            TextMessages.From(Build("BUM", null, NativeNewLine, V, N1)), db);
        Check.True(dto is null, "A BUM without .F must not parse.");
        Check.True(db.MessageErrors.Local.Any(), "An error row is recorded.");
    }

    [Fact(DisplayName = "Spec 8.7.2: a BCM flight line without a destination is accepted (the destination is optional)")]
    [Trait("Kind", "Spec")]
    public void Bcm_flight_without_destination()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BcmFomParser(ElementMapFactory.Create()).ProcessMessage(
            TextMessages.From(BcmText("FOM", V, ".F/LH3348/20MAR")), db);
        Check.True(dto is not null, "Destination is optional in the FOM, FCM and DBM.");
    }

    [Theory(DisplayName = "Dates: day 01 to 31 and a known month are accepted, 00 and 32 are not")]
    [Trait("Kind", "Spec")]
    [InlineData(".F/LH400/01NOV/JFK", true)]
    [InlineData(".F/LH400/31DEC/JFK", true)]
    [InlineData(".F/LH400/00NOV/JFK", false)]
    [InlineData(".F/LH400/32NOV/JFK", false)]
    public void Date_day_range(string line, bool expected)
        => Check.True(IsValid(new OutboundFlightElementValidator(), line) == expected, $"{line} expected valid={expected}.");

    [Fact(DisplayName = "Spec 8.7.2: a FOM may repeat .R")]
    [Trait("Kind", "Spec")]
    public void Fom_repeats_r()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BcmFomParser(ElementMapFactory.Create()).ProcessMessage(
            TextMessages.From(BcmText("FOM", V, ".F/LH400/12NOV/JFK", ".R/FIRST", ".R/SECOND")), db);
        Check.True(dto is not null, "The message follows the standard and must parse.");
    }

    [Theory(DisplayName = "BCM: the parsed result is stored as a ParsedMessages row whose message type is the routing key (BCMFOM, BCMFCM, BCMDBM)")]
    [Trait("Bug", "14")]
    [InlineData("FOM", "BCMFOM")]
    [InlineData("FCM", "BCMFCM")]
    [InlineData("DBM", "BCMDBM")]
    public void Bcm_is_stored_in_parsed_messages(string subType, string expectedType)
    {
        var map = ElementMapFactory.Create();
        MessageParser<Dtos.BcmDto> parser = subType switch
        {
            "FOM" => new BcmFomParser(map),
            "FCM" => new BcmFcmParser(map),
            _ => new BcmDbmParser(map)
        };
        var message = TextMessages.From(BcmText(subType, V, ".F/LH400/12NOV/JFK"));
        var result = parser.Parse(message).Value;
        var parsed = ParsedMessageFactory.Create(message, result!);

        Check.True(parsed is not null, "A BCM result must be mapped.");
        Check.Equal(expectedType, parsed!.MessageType, "Message type.");
        Check.Equal("LH", parsed.AirlineCode, "Airline code.");
        Check.Equal("400", parsed.FlightNumber, "Flight number.");
    }

    [Fact(DisplayName = "BRS policy: an invalid element that BRS does not use is skipped with a warning and the message is still processed")]
    [Trait("Kind", "Policy")]
    public void Invalid_non_essential_element_does_not_reject()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, N1, ".E/R1", ".L/AB", P)), db);
        Check.True(dto is not null, "Bad .E and .L lines must not reject the message.");
        Check.True(db.ElementErrors.Local.Any(e => e.ErrorDescription!.Contains("ElementE")), "The skipped element is recorded as a warning.");
    }

    [Fact(DisplayName = "BRS policy: an invalid essential element (.N) still rejects the message")]
    [Trait("Kind", "Policy")]
    public void Invalid_essential_element_rejects()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, ".N/022012345")), db);
        Check.True(dto is null, "A bad bag tag line must reject the message.");
    }

    [Fact(DisplayName = "BRS policy: a repeated element that BRS does not use is ignored instead of rejecting the message")]
    [Trait("Kind", "Policy")]
    public void Duplicate_non_essential_element_does_not_reject()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, N1, ".Y/AB123", ".Y/CD456", P)), db);
        Check.True(dto is not null, "A second .Y must not reject the message.");
    }

    [Fact(DisplayName = "Spec 2.3.9: flight lines without a class are accepted and their class is null, not the default char")]
    [Trait("Kind", "Spec")]
    public void Classless_flights_have_null_class()
    {
        using var db = TestDbContextFactory.Create();
        var text = BsmText(V, ".F/LH400/12NOV/JFK", ".I/AZ318/11NOV/FCO", ".O/SR1234/18APR/ATL", N1, P);
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(text), db);
        Check.True(dto is not null, "Class is optional on .F, .I and .O.");
        Check.True(dto!.PassengerClass is null, "No class on .F means null.");
        Check.True(dto.Onwards.Single().Class is null, "No class on .O means null.");
    }

    [Theory(DisplayName = "Spec 2.3.9: flight numbers are 1 to 4 digits with an optional trailing letter (the standard examples use US14)")]
    [Trait("Kind", "Spec")]
    [InlineData(".F/LH400/12NOV/JFK", true)]
    [InlineData(".F/LH4000/12NOV/JFK", true)]
    [InlineData(".F/LH400A/12NOV/JFK", true)]
    [InlineData(".F/LH40/12NOV/JFK", true)]
    [InlineData(".F/LH40000/12NOV/JFK", false)]
    public void Flight_number_format(string line, bool expected)
        => Check.True(IsValid(new OutboundFlightElementValidator(), line) == expected, $"{line} expected valid={expected}.");

    [Fact(DisplayName = "Spec 6.9.3: the one-bag BUM example of the standard parses")]
    [Trait("Kind", "Spec")]
    public void Bum_single_bag_example()
    {
        using var db = TestDbContextFactory.Create();
        var text = Build("BUM", null, NativeNewLine, ".V/1LPIT////123ABC456Z", ".F/US14/01MAY/MUC/J",
            ".U/AVE0321US/0012/T/J/MUC", ".N/0037336127001", ".Q/026", ".P/1BROWNE/L");
        var dto = new BumParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(text), db);
        Check.True(dto is not null, "Must parse. " + string.Join("; ", db.MessageErrors.Local.Select(e => e.ErrorCode + " " + e.ErrorDescription).Concat(db.ElementErrors.Local.Select(e => e.ErrorCode + " " + e.ErrorDescription))));
    }
}

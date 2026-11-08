using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Parsers.Messages.Bcms;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Requests;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class MessageTests
{
    [Fact(DisplayName = "Baseline: single-tag BSM is parsed into the expected request DTO")]
    [Trait("Kind", "Baseline")]
    public void Single_tag_bsm_parses()
    {
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(SingleTagBsm(), 7), db);
        Check.True(dto is not null, "A well-formed single-tag BSM must parse.");
        Check.Equal(7L, dto!.Id, "Message id is copied.");
        Check.Equal('L', dto.SourceIndicator, "Source indicator from .V/7LFRA.");
        Check.Equal("FRA", dto.SourceAirportCode, "Source airport from .V/7LFRA.");
        Check.Equal("LH", dto.AirlineCode, "Airline from .F/LH400.");
        Check.Equal("400", dto.FlightNumber, "Flight number from .F/LH400.");
        Check.Equal("12NOV", dto.FlightIataDate, "Date from .F.");
        Check.Equal("JFK", dto.Destination, "Destination from .F.");
        Check.Equal((char?)'Y', dto.PassengerClass, "Class from .F.");
        Check.SequenceEqual(new[] { "0220123456" }, dto.BagTags, "One bag tag from .N/0220123456001.");
        Check.Equal("JOHN DOE", dto.PassengerName, "Name from .P/DOE/JOHN.");
        Check.Equal(0, db.MessageErrors.Local.Count + db.ElementErrors.Local.Count, "No error rows for a valid message.");
    }

    [Fact(DisplayName = "Baseline: BSM CHG and BSM DEL with one tag parse into their DTOs")]
    [Trait("Kind", "Baseline")]
    public void Chg_and_del_parse()
    {
        var map = ElementMapFactory.Create();
        using var db = TestDbContextFactory.Create();
        var chg = new BsmChgParser(map).ProcessMessage(TextMessages.From(ChgText(V, F, N1, P)), db);
        var del = new BsmDelParser(map).ProcessMessage(TextMessages.From(DelText(V, F, N1)), db);
        Check.True(chg is not null && chg.BagTags.SequenceEqual(new[] { "0220123456" }), "CHG parse.");
        Check.True(del is not null && del.BagTags.SequenceEqual(new[] { "0220123456" }), "DEL parse.");
    }

    [Fact(DisplayName = "Bug 3: BSM with three .N lines (serials 123456, 123457, 123458) must yield three distinct bag tags")]
    [Trait("Bug", "3")]
    public void Three_n_lines_give_three_tags()
    {
        const string Why = "All three .N lines are parsed by the same shared ElementN instance and the DTO is built only after all parsing is done, so every ElementResult points to the same object holding the LAST serial. Expected 0220123456, 0220123457, 0220123458.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N1, N2, N3, P)), db);
        Check.True(dto is not null, "Message must parse. " + Why);
        Check.SequenceEqual(new[] { "0220123456", "0220123457", "0220123458" }, dto!.BagTags, Why);
    }

    [Fact(DisplayName = "Bug 3: BSM with two .O onward lines must keep both onward flights")]
    [Trait("Bug", "3")]
    public void Two_o_lines_give_two_distinct_onwards()
    {
        const string Why = "Same shared-instance problem for ElementO: both onward entries end up equal to the last .O line.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(BsmText(V, F, ".O/LH401/13NOV/MUC/Y", ".O/LH402/14NOV/BER/Y", N1, P)), db);
        Check.True(dto is not null, "Message must parse. " + Why);
        Check.SequenceEqual(new[] { "MUC", "BER" }, dto!.Onwards.Select(o => o.Destination), Why);
    }

    [Fact(DisplayName = "Bug 3: ElementP.AdditionalGivenNames, passenger count and given name must not leak from message A into message B")]
    [Trait("Bug", "3")]
    public void Element_p_state_does_not_leak_between_messages()
    {
        const string Why = "ElementP is shared and stateful: AdditionalGivenNames is only ever appended to, NumberOfPassengersWithSurname and PassengerGivenName are only overwritten when present. Message B (.P/ROE only) must not see MIDDLE1/MIDDLE2, '2' or JOHN from message A.";
        var map = ElementMapFactory.Create();
        var bsm = new BsmParser(map);
        using var db = TestDbContextFactory.Create();
        bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N1, ".P/2DOE/JOHN/MIDDLE1/MIDDLE2"), 1), db);
        bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N2, ".P/ROE"), 2), db);
        var p = ElementMapFactory.Get<ElementP>(map);
        Check.Equal(0, p.AdditionalGivenNames.Count, "AdditionalGivenNames leaked: " + string.Join(",", p.AdditionalGivenNames) + ". " + Why);
        Check.Equal(string.Empty, p.NumberOfPassengersWithSurname, "NumberOfPassengersWithSurname leaked. " + Why);
        Check.Equal(string.Empty, p.PassengerGivenName, "PassengerGivenName leaked. " + Why);
    }

    [Fact(DisplayName = "Bug 3: DTO of message B must not contain the given name of message A when B has only a surname")]
    [Trait("Bug", "3")]
    public void Passenger_name_does_not_leak_into_dto()
    {
        const string Why = "Message B has .P/ROE (no given name). The DTO name must not be 'JOHN ROE' with JOHN taken from message A.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N1, P), 1), db);
        var b = bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N2, ".P/ROE"), 2), db);
        Check.True(b is not null, "Message B must parse.");
        Check.True(!b!.PassengerName.Contains("JOHN"), $"Name was '{b.PassengerName}'. {Why}");
    }

    [Fact(DisplayName = "Bug 3 (probe): seat number from message A must not appear in message B whose .S has no seat")]
    [Trait("Bug", "3")]
    public void Element_s_seat_does_not_leak()
    {
        const string Why = "ElementS.Parse assigns every property on every call (using default when a field is absent), so a leak is not expected here. Written as a probe; green means ElementS is NOT affected.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N1, ".S/Y/12A/B/001/123/Y/N/A"), 1), db);
        var b = bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N2, ".S/Y"), 2), db);
        Check.True(b is not null, "Message B must parse.");
        Check.True(b!.SeatNumber is null, $"Seat was '{b.SeatNumber}'. {Why}");
    }

    [Fact(DisplayName = "Bug 3: ElementW unit and dimensions from message A must not remain after message B that carries only weight")]
    [Trait("Bug", "3")]
    public void Element_w_state_does_not_leak()
    {
        const string Why = "ElementW.Parse only assigns Unit/Length/Width/Height/BaggageTypeCode inside the 'is unit' branch and UncheckedWeight in the else branch; fields of the previous message survive.";
        var map = ElementMapFactory.Create();
        var bsm = new BsmParser(map);
        using var db = TestDbContextFactory.Create();
        bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N1, ".W/K/001/20/CM/55/40/20/XYZ"), 1), db);
        bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N2, ".W/K/001/20"), 2), db);
        var w = ElementMapFactory.Get<ElementW>(map);
        Check.Equal(string.Empty, w.Unit, "Unit leaked. " + Why);
        Check.Equal(string.Empty, w.LengthOfBag, "LengthOfBag leaked. " + Why);
    }

    [Fact(DisplayName = "Bug 3: many concurrent messages on one shared BsmParser instance must each return their own bag tag and passenger")]
    [Trait("Bug", "3")]
    public async Task Concurrent_processing_does_not_mix_results()
    {
        const string Why = "Element instances (and their private-set properties) are shared by all concurrent messages, so results are mixed.";
        const int Parallelism = 32;
        const int Rounds = 20;
        var bsm = new BsmParser(ElementMapFactory.Create());
        var mismatches = new System.Collections.Concurrent.ConcurrentBag<string>();
        for (var round = 0; round < Rounds; round++)
        {
            using var barrier = new Barrier(Parallelism);
            var tasks = Enumerable.Range(0, Parallelism).Select(i => Task.Factory.StartNew(() =>
            {
                var suffix = string.Concat(i.ToString().Select(c => (char)('A' + (c - '0'))));
                var raw = BsmText(V, F, $".N/0220{i + 100:D6}001", $".P/DOE{suffix}/NAME{suffix}");
                var message = TextMessages.From(raw, i);
                using var db = TestDbContextFactory.Create();
                barrier.SignalAndWait();
                var dto = bsm.ProcessMessage(message, db);
                var expectedTag = $"0220{i + 100:D6}";
                if (dto is null || !dto.BagTags.SequenceEqual(new[] { expectedTag }) || dto.PassengerName != $"NAME{suffix} DOE{suffix}")
                    mismatches.Add($"msg {i}: tags=[{string.Join(",", dto?.BagTags ?? [])}] name={dto?.PassengerName}");
            }, TaskCreationOptions.LongRunning)).ToArray();
            await Task.WhenAll(tasks);
        }
        Check.True(mismatches.IsEmpty, $"{mismatches.Count} of {Parallelism * Rounds} messages got another message's data (first: {mismatches.FirstOrDefault()}). {Why}");
    }

    [Theory(DisplayName = "Bug 4: BSM with malformed Conditional .F line must produce a validation error result, not an unhandled index exception")]
    [Trait("Bug", "4")]
    [InlineData(".F/")]
    [InlineData(".F/LH400")]
    [InlineData(".F/LH400/12NOV")]
    [InlineData(".F/XX/YY/ZZ/Q")]
    public void Malformed_conditional_f_gives_error_not_exception(string fLine)
    {
        const string Why = "In a BSM, F is Conditional so ElementFValidator skips all checks, then ElementF.Parse indexes ParsedText[1].Span[2] / ParsedText[2] / [3] and throws. Expected: ProcessMessage returns null and an error row is recorded.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        Dtos.DepartureBagDto? dto = null;
        var ex = Record.Exception(() => dto = bsm.ProcessMessage(TextMessages.From(BsmText(V, fLine, N1, P)), db));
        Check.True(ex is null, $"'{fLine}' threw {ex?.GetType().Name}: {ex?.Message}. {Why}");
        Check.True(dto is null, $"'{fLine}' produced a DTO. {Why}");
        Check.True(db.ElementErrors.Local.Count + db.MessageErrors.Local.Count > 0, $"No error recorded for '{fLine}'. {Why}");
    }

    private const string StatusProfileS = ".S/Y/12A/B/001/123/Y/N/A";

    [Fact(DisplayName = "Bug 6: BsmParser DTO PassengerStatus must be the S passenger status (B), not the profile status (Y)")]
    [Trait("Bug", "6")]
    public void Bsm_passenger_status_mapping()
    {
        const string Why = "In .S/Y/12A/B/001/123/Y/N/A field 3 is passenger status 'B' and field 6 is profile status 'Y'. BsmParser assigns elementS.PassengerProfileStatus to the DTO's PassengerStatus argument and never uses passengerStatus.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(BsmText(V, F, N1, StatusProfileS, P)), db);
        Check.True(dto is not null, "Message must parse.");
        Check.Equal('B', dto!.PassengerStatus, Why);
    }

    [Fact(DisplayName = "Bug 6: BsmChgParser DTO PassengerStatus must be the S passenger status (B), not the profile status (Y)")]
    [Trait("Bug", "6")]
    public void BsmChg_passenger_status_mapping()
    {
        const string Why = "Same mapping error in BsmChgParser.";
        var bsm = new BsmChgParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var dto = bsm.ProcessMessage(TextMessages.From(ChgText(V, F, N1, StatusProfileS, P)), db);
        Check.True(dto is not null, "Message must parse.");
        Check.Equal('B', dto!.PassengerStatus, Why);
    }

    [Theory(DisplayName = "Bug 6: BSM DTOs must expose the W (weight) data that the message carries")]
    [Trait("Bug", "6")]
    [InlineData(typeof(DepartureBagDto))]
    public void Dto_exposes_weight(Type dtoType)
    {
        Check.True(dtoType.GetProperties().Any(p => p.Name.Contains("Weight", StringComparison.OrdinalIgnoreCase)),
            $"{dtoType.Name} has no weight property. BsmParser/BsmChgParser read PiecesWeightIndicator and CheckedWeight from ElementW into locals that are never used, so .W/K/001/20 data is lost.");
    }

    [Fact(DisplayName = "Bug 6: the W element values of a BSM and a BSM CHG reach the DTO (.W/K/001/20 gives indicator K, 001 bags, 20 kg)")]
    [Trait("Bug", "6")]
    public void Weight_values_reach_the_dto()
    {
        const string W = ".W/K/001/20";
        using var db = TestDbContextFactory.Create();
        var create = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, N1, W, P)), db);
        var change = new BsmChgParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(ChgText(V, F, N1, W, P)), db);
        foreach (var weight in new[] { create?.BaggageWeight, change?.BaggageWeight })
        {
            Check.True(weight is not null, "BaggageWeight must be filled when the message carries a .W/ element.");
            Check.Equal("K", weight!.PiecesWeightIndicator, "Pieces/weight indicator.");
            Check.Equal("001", weight.NumberOfCheckedBag, "Number of checked bags.");
            Check.Equal("20", weight.CheckedWeight, "Checked weight.");
        }
    }

    [Fact(DisplayName = "Bug 6: a BSM without a W element has no BaggageWeight, and the S profile status reaches the DTO separately from the passenger status")]
    [Trait("Bug", "6")]
    public void Absent_weight_is_null_and_profile_status_is_kept()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, N1, StatusProfileS, P)), db);
        Check.True(dto is not null, "Message must parse.");
        Check.True(dto!.BaggageWeight is null, "No .W/ element was sent.");
        Check.Equal('B', dto.PassengerStatus, "Passenger status is field 3.");
        Check.Equal('Y', dto.PassengerProfileStatus, "Passenger profile status is field 6 and must not be lost.");
    }

    [Fact(DisplayName = "Bug 5/6: a .S/ line with empty optional fields (.S/Y//S/291) parses without exceptions and keeps the empty seat as null")]
    [Trait("Bug", "6")]
    public void Empty_optional_s_fields_do_not_crash_the_parser()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, N1, ".S/Y//S/291", P)), db);
        Check.True(dto is not null, "The specification's own example must parse.");
        Check.True(dto!.SeatNumber is null, "An empty seat field must be null.");
        Check.Equal('S', dto.PassengerStatus, "Passenger status S (stand-by).");
        Check.Equal("291", dto.PassengerSequenceNumber, "Sequence number.");
    }

    [Theory(DisplayName = "Bug 7: BSM containing an unknown element line must yield UNKNOWN_ELEMENT_FOUND, not InvalidOperationException")]
    [Trait("Bug", "7")]
    [InlineData(".Z/XYZ")]
    [InlineData("THIS IS FREE TEXT")]
    [InlineData("AB")]
    [InlineData(".U/CONT1/1/M/Y/FRA/N")]
    public void Unknown_element_gives_validation_error(string unknownLine)
    {
        const string Why = "SplitSpanMessage resolves each line with elementSequences.First(...) which throws InvalidOperationException for an identifier that is not in the message's sequence list, so the UNKNOWN_ELEMENT_FOUND branch in ValidateMessage is unreachable.";
        var bsm = new BsmParser(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        Dtos.DepartureBagDto? dto = null;
        var ex = Record.Exception(() => dto = bsm.ProcessMessage(TextMessages.From(BsmText(V, F, unknownLine, N1, P)), db));
        Check.True(ex is null, $"'{unknownLine}' threw {ex?.GetType().Name}: {ex?.Message}. {Why}");
        Check.True(dto is null, "A message with an unknown element must not produce a DTO.");
        Check.True(db.MessageErrors.Local.Any(e => e.ErrorCode == "UNKNOWN_ELEMENT_FOUND"), $"No UNKNOWN_ELEMENT_FOUND error recorded for '{unknownLine}'. {Why}");
    }

    [Theory(DisplayName = "Bug 11: BCM subtype is reported in the DTO (FOM->Fom, FCM->Fcm, DBM->Dbm)")]
    [Trait("Bug", "11")]
    [InlineData("FOM", "Fom")]
    [InlineData("FCM", "Fcm")]
    [InlineData("DBM", "Dbm")]
    public void Bcm_subtype(string subType, string expected)
    {
        const string Why = "BcmDbmParser.ToBcmFomDto hard-codes \"Fcm\" (copy-paste from BcmFcmParser). FOM and FCM are regression checks and pass.";
        var map = ElementMapFactory.Create();
        MessageParser<BcmDto> handler = subType switch
        {
            "FOM" => new BcmFomParser(map),
            "FCM" => new BcmFcmParser(map),
            _ => new BcmDbmParser(map)
        };
        using var db = TestDbContextFactory.Create();
        var dto = handler.ProcessMessage(TextMessages.From(BcmText(subType, V, ".F/LH400/12NOV/JFK")), db);
        Check.True(dto is not null, "BCM must parse.");
        Check.Equal(expected, dto!.SubType, Why);
    }
}

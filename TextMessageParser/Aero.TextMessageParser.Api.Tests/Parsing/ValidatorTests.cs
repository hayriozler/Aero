using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Elements.Validators;
using Aero.TextMessageParser.Api.Tests.Infrastructure;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class ValidatorTests
{
    private static IElementValidator ValidatorFor(char id) => id switch
    {
        'F' => new ElementFValidator(),
        'I' => new ElementIValidator(),
        'O' => new ElementOValidator(),
        'P' => new ElementPValidator(),
        'S' => new ElementSValidator(),
        'U' => new ElementUValidator(),
        'W' => new ElementWValidator(),
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };

    [Theory(DisplayName = "Bug 4 (control): a clearly malformed element IS rejected when its requirement is Mandatory")]
    [Trait("Kind", "Baseline")]
    [InlineData('F', ".F/")]
    [InlineData('I', ".I/")]
    [InlineData('O', ".O/")]
    [InlineData('P', ".P/")]
    [InlineData('S', ".S/")]
    [InlineData('U', ".U/")]
    [InlineData('W', ".W/")]
    public void Malformed_element_is_rejected_when_mandatory(char id, string line)
    {
        var result = ValidatorFor(id).Validate(ElementDetails.Make(line, ElementRequirement.Mandatory));
        Check.True(!result.IsValid, $"Control: {line} is an empty element that every validator rejects by length when Mandatory. If this fails the sample is not malformed enough for the Bug 4 tests.");
    }

    [Theory(DisplayName = "Bug 4: a clearly malformed element must be rejected even when its requirement is Conditional or Optional")]
    [Trait("Bug", "4")]
    [InlineData('F', ".F/", ElementRequirement.Conditional)]
    [InlineData('F', ".F/", ElementRequirement.Optional)]
    [InlineData('I', ".I/", ElementRequirement.Conditional)]
    [InlineData('I', ".I/", ElementRequirement.Optional)]
    [InlineData('O', ".O/", ElementRequirement.Conditional)]
    [InlineData('O', ".O/", ElementRequirement.Optional)]
    [InlineData('P', ".P/", ElementRequirement.Conditional)]
    [InlineData('P', ".P/", ElementRequirement.Optional)]
    [InlineData('S', ".S/", ElementRequirement.Conditional)]
    [InlineData('S', ".S/", ElementRequirement.Optional)]
    [InlineData('U', ".U/", ElementRequirement.Conditional)]
    [InlineData('U', ".U/", ElementRequirement.Optional)]
    [InlineData('W', ".W/", ElementRequirement.Conditional)]
    [InlineData('W', ".W/", ElementRequirement.Optional)]
    public void Malformed_element_is_rejected_regardless_of_requirement(char id, string line, ElementRequirement requirement)
    {
        const string Why = "Every validator begins with 'if (Requirement != Mandatory) return valid'. Optional and Conditional only mean the element may be absent; when present it still has to be well formed. Otherwise the parser later indexes into missing fields.";
        var result = ValidatorFor(id).Validate(ElementDetails.Make(line, requirement));
        Check.True(!result.IsValid, $"Validator {id} accepted '{line}' with requirement {requirement}. {Why}");
    }

    [Theory(DisplayName = "Bug 5: ElementS accepts every well-formed .S/ line allowed by IATA Passenger Services Conference Resolutions Manual (30th ed.) section 2.3.22, including empty optional fields")]
    [Trait("Bug", "5")]
    [InlineData(".S/Y")]
    [InlineData(".S/N/4K")]
    [InlineData(".S/Y//S/291")]
    [InlineData(".S/Y/23A/C/016/678/Y/Y")]
    [InlineData(".S/Y/23A/C/016/678/Y/Y/I")]
    [InlineData(".S/Y/23A/C/016/678/N/N/A")]
    [InlineData(".S/Y/23A/B")]
    public void ElementS_valid_lines_pass(string line)
    {
        const string Why = "Section 2.3.22: Authority to Load (Y/N) is the only mandatory field. Seat is f(f)a, passenger status B/C/N/S, sequence and security number are three digits, profile and transport authority Y/N, baggage tag status I/A. Every field after the first may be empty or omitted, as in the specification examples '.S/Y//S/291' and '.S/Y'.";
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Conditional));
        Check.True(result.IsValid, $"'{line}' was rejected with {result.ErrorCode} ({result.Description}). {Why}");
    }

    [Theory(DisplayName = "Bug 5: ElementS rejects a value that breaks the format of its field (position follows ElementS.Parse and the specification)")]
    [Trait("Bug", "5")]
    [InlineData(".S/B", "S_04")]
    [InlineData(".S/Q", "S_04")]
    [InlineData(".S/Y/123", "S_05")]
    [InlineData(".S/Y/A1", "S_05")]
    [InlineData(".S/Y/1234A", "S_05")]
    [InlineData(".S/Y/23A/Q", "S_01")]
    [InlineData(".S/Y/23A/C/01", "S_06")]
    [InlineData(".S/Y/23A/C/0AB", "S_06")]
    [InlineData(".S/Y/23A/C/016/67", "S_07")]
    [InlineData(".S/Y/23A/C/016/678/Q", "S_02")]
    [InlineData(".S/Y/23A/C/016/678/Y/Q", "S_08")]
    [InlineData(".S/Y/23A/C/016/678/Y/Y/Z", "S_03")]
    public void ElementS_invalid_values_fail(string line, string expectedCode)
    {
        const string Why = "Each field has its own format in section 2.3.22. The old validator compared the wrong positions, used an inverted condition and only looked at lines with exactly 2, 5 or 6 fields.";
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Conditional));
        Check.True(!result.IsValid, $"'{line}' was accepted. {Why}");
        Check.Equal(expectedCode, result.ErrorCode, $"'{line}' was rejected with the wrong code. {Why}");
    }

    [Theory(DisplayName = "Bug 5: ElementS rejects lines outside the 6 to 26 character range of the specification (the count includes the 2 character end delimiter)")]
    [Trait("Bug", "5")]
    [InlineData(".S/")]
    [InlineData(".S/Y/23A/C/016/678/Y/Y/A/X")]
    public void ElementS_length_is_enforced(string line)
    {
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Conditional));
        Check.True(!result.IsValid, $"'{line}' is outside the 6 to 26 character range of section 2.3.22 and must be rejected.");
    }

    [Theory(DisplayName = "Baseline: SplitOnSlash keeps empty fields and counts slashes + 1 fields")]
    [Trait("Kind", "Baseline")]
    [InlineData(".V/1LFRA/001/REF", 4)]
    [InlineData(".P/DOE//JOHN", 4)]
    [InlineData(".F/", 2)]
    [InlineData("ABC", 1)]
    public void SplitOnSlash_counts(string line, int expected)
    {
        Check.Equal(expected, ParserExtensions.SplitOnSlash(line.AsMemory()).Length, $"Field count for '{line}'.");
    }

    [Theory(DisplayName = "Baseline: ValidationHelper flight number, IATA date and airport code rules")]
    [Trait("Kind", "Baseline")]
    [InlineData("LH400", true)]
    [InlineData("LH1234", true)]
    [InlineData("LH", false)]
    [InlineData("LH12345", false)]
    [InlineData("LHABCD", false)]
    public void ValidateAirlineFlightNumber(string value, bool expected)
    {
        Check.Equal(expected, ValidationHelper.ValidateAirlineFlightNumber(value.AsMemory(), out _), $"Flight '{value}'.");
    }

    [Theory(DisplayName = "Baseline: ValidationHelper IATA date")]
    [Trait("Kind", "Baseline")]
    [InlineData("12NOV", true)]
    [InlineData("01JAN", true)]
    [InlineData("1NOV", false)]
    [InlineData("12XXX", false)]
    [InlineData("AANOV", false)]
    public void ValidateIataDate(string value, bool expected)
    {
        Check.Equal(expected, ValidationHelper.ValidateIataDate(value.AsMemory()), $"Date '{value}'.");
    }

    [Theory(DisplayName = "Baseline: ValidationHelper airport code")]
    [Trait("Kind", "Baseline")]
    [InlineData("JFK", true)]
    [InlineData("JF", false)]
    [InlineData("J1K", false)]
    [InlineData("JFKX", false)]
    public void ValidateAirportCode(string value, bool expected)
    {
        Check.Equal(expected, ValidationHelper.ValidateAirportCode(value.AsMemory()), $"Airport '{value}'.");
    }
}

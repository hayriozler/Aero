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

    [Theory(DisplayName = "Bug 5: ElementS validator accepts valid values at the positions it checks (length 2, 5 and 6 branches)")]
    [Trait("Bug", "5")]
    [InlineData(".S/B")]
    [InlineData(".S/Y/12A/C/Y")]
    [InlineData(".S/Y/12A/C/001/I")]
    [InlineData(".S/Y/12A/C/001/A")]
    public void ElementS_valid_values_pass(string line)
    {
        const string Why = "The branches use 'if (array.ContainsSpan(value)) AddError(...)': the condition is inverted, so a value from the allowed list is reported as an error. Note the validator's field positions (1, 4, 5) also disagree with ElementS.Parse (status 3, profile 6, bag tag status 8).";
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Mandatory));
        Check.True(result.IsValid, $"'{line}' was rejected with {result.ErrorCode} ({result.Description}). {Why}");
    }

    [Theory(DisplayName = "Bug 5: ElementS validator rejects invalid values at the positions it checks (length 2, 5 and 6 branches)")]
    [Trait("Bug", "5")]
    [InlineData(".S/Q")]
    [InlineData(".S/Y/12A/C/Q")]
    [InlineData(".S/Y/12A/C/001/Z")]
    public void ElementS_invalid_values_fail(string line)
    {
        const string Why = "Same inverted condition: values outside the allowed list sail through validation.";
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Mandatory));
        Check.True(!result.IsValid, $"'{line}' was accepted. {Why}");
    }

    [Theory(DisplayName = "Bug 5: ElementS validator rejects an invalid value in every field count that has no switch case (3, 4, 7, 8 and 9 fields)")]
    [Trait("Bug", "5")]
    [InlineData(".S/Q/12A")]
    [InlineData(".S/Y/12A/Q")]
    [InlineData(".S/Y/12A/B/001/SEC/Q")]
    [InlineData(".S/Y/12A/B/001/SEC/Y/Q")]
    [InlineData(".S/Y/12A/B/001/SEC/Y/N/Z")]
    public void ElementS_unchecked_lengths_reject_invalid_values(string line)
    {
        const string Why = "The switch only handles ParsedText lengths 2, 5 and 6, so elements with 3, 4, 7, 8 or 9 slash-separated parts are never value-checked. Invalid authority-to-load, passenger status, profile, authority-to-transport or bag-tag status passes.";
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Mandatory));
        Check.True(!result.IsValid, $"'{line}' was accepted. {Why}");
    }

    [Theory(DisplayName = "Bug 5 (control): ElementS validator accepts fully valid elements of 3, 4, 7, 8 and 9 fields")]
    [Trait("Kind", "Baseline")]
    [InlineData(".S/Y/12A")]
    [InlineData(".S/Y/12A/B")]
    [InlineData(".S/Y/12A/B/001/SEC/Y")]
    [InlineData(".S/Y/12A/B/001/SEC/Y/N")]
    [InlineData(".S/Y/12A/B/001/SEC/Y/N/A")]
    public void ElementS_valid_lengths_pass(string line)
    {
        var result = new ElementSValidator().Validate(ElementDetails.Make(line, ElementRequirement.Mandatory));
        Check.True(result.IsValid, $"'{line}' must stay valid after the validator is fixed ({result.ErrorCode}).");
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

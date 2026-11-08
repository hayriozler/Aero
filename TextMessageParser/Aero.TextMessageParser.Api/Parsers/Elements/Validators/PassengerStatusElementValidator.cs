using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

public class PassengerStatusElementValidator : IElementValidator
{
    private const int MaxFieldCount = 9;

    public ElementValidationResult Validate(ElementDetail elementDetail)
    {
        var validationResult = new ElementValidationResult(elementDetail);
        if (elementDetail.Length < 6 || elementDetail.Length > 26)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "PassengerStatusElement length wrong");
            return validationResult;
        }
        var fields = elementDetail.ParsedText;
        if (fields.Length < 2 || fields.Length > MaxFieldCount)
        {
            validationResult.AddError(ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "PassengerStatusElement field count wrong");
            return validationResult;
        }
        if (!Consts.AUTHORITY_LOAD_ARRAY.ContainsSpan(fields[1].Span))
        {
            validationResult.AddError(ErrorCodes.AUTHORITY_TO_LOAD_VALUE_NOT_CORRECT, "Authority to load must be Y or N");
            return validationResult;
        }
        if (fields.Length > 2 && !IsValidSeat(fields[2].Span))
        {
            validationResult.AddError(ErrorCodes.SEAT_NUMBER_NOT_CORRECT, "Seat number must be one or two digits followed by a letter");
            return validationResult;
        }
        if (fields.Length > 3 && !IsEmptyOrOneOf(fields[3], Consts.PASSENGER_STATUS_ARRAY))
        {
            validationResult.AddError(ErrorCodes.PASSENGER_STATUS_VALUE_NOT_CORRECT, "Passenger status must be B, C, N or S");
            return validationResult;
        }
        if (fields.Length > 4 && !IsEmptyOrDigits(fields[4].Span, 3))
        {
            validationResult.AddError(ErrorCodes.SEQUENCE_NUMBER_NOT_CORRECT, "Sequence number must be three digits");
            return validationResult;
        }
        if (fields.Length > 5 && !IsEmptyOrDigits(fields[5].Span, 3))
        {
            validationResult.AddError(ErrorCodes.SECURITY_NUMBER_NOT_CORRECT, "Security number must be three digits");
            return validationResult;
        }
        if (fields.Length > 6 && !IsEmptyOrOneOf(fields[6], Consts.YesNoValues))
        {
            validationResult.AddError(ErrorCodes.PASSENGER_PROFIL_STATUS_VALUE_NOT_CORRECT, "Passenger profile status must be Y or N");
            return validationResult;
        }
        if (fields.Length > 7 && !IsEmptyOrOneOf(fields[7], Consts.YesNoValues))
        {
            validationResult.AddError(ErrorCodes.AUTHORITY_TO_TRANSPORT_VALUE_NOT_CORRECT, "Authority to transport must be Y or N");
            return validationResult;
        }
        if (fields.Length > 8 && !IsEmptyOrOneOf(fields[8], Consts.BagTagStatusValues))
        {
            validationResult.AddError(ErrorCodes.BAGTAG_STATUS_VALUE_NOT_CORRECT, "Baggage tag status must be I or A");
            return validationResult;
        }
        return validationResult;
    }

    private static bool IsEmptyOrOneOf(ReadOnlyMemory<char> field, string[] allowedValues)
        => field.IsEmpty || allowedValues.ContainsSpan(field.Span);

    private static bool IsEmptyOrDigits(ReadOnlySpan<char> field, int length)
    {
        if (field.IsEmpty) return true;
        if (field.Length != length) return false;
        foreach (var c in field)
            if (!char.IsDigit(c)) return false;
        return true;
    }

    private static bool IsValidSeat(ReadOnlySpan<char> seat)
    {
        if (seat.IsEmpty) return true;
        if (seat.Length < 2 || seat.Length > 3) return false;
        if (!char.IsLetter(seat[^1])) return false;
        for (var i = 0; i < seat.Length - 1; i++)
            if (!char.IsDigit(seat[i])) return false;
        return true;
    }
}

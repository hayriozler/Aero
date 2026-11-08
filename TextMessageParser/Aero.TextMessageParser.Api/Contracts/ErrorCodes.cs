namespace Aero.TextMessageParser.Api.Contracts;

public static class ErrorCodes
{
    public const string NO_MSG_REGISTERED_HANDLER = "MSG_HND_01";
    public const string UNHANDLED_PROCESSING_ERROR = "MSG_HND_02";
    public const string UNKNOWN_ELEMENT_FOUND = "UNKNOWN_ELEMENT_FOUND";

    public const string ELEMENT_LENGTH_NOT_CORRECT = "1001";
    public const string ELEMENT_VALUE_NOT_CORRECT = "1002";
    public const string ELEMENT_FORMAT_NOT_CORRECT = "1003";
    public const string ELEMENT_FIELD_MISSING = "1004";

    public const string BAGGAGE_SOURCE_INDICATOR_NOT_CORRECT = "V_01";
    public const string ACK_NAK_VALUE_NOT_CORRECT = "A_01";
    public const string BAG_STATUS_VALUE_NOT_CORRECT = "B_01";
    public const string OUTBOUND_FLIGHT_NOT_CORRECT = "F_01";

    public const string PASSENGER_STATUS_VALUE_NOT_CORRECT = "S_01";
    public const string PASSENGER_PROFIL_STATUS_VALUE_NOT_CORRECT = "S_02";
    public const string BAGTAG_STATUS_VALUE_NOT_CORRECT = "S_03";
    public const string AUTHORITY_TO_LOAD_VALUE_NOT_CORRECT = "S_04";
    public const string SEAT_NUMBER_NOT_CORRECT = "S_05";
    public const string SEQUENCE_NUMBER_NOT_CORRECT = "S_06";
    public const string SECURITY_NUMBER_NOT_CORRECT = "S_07";
    public const string AUTHORITY_TO_TRANSPORT_VALUE_NOT_CORRECT = "S_08";

    public static string MessageEmpty(string messageName) => $"MSG_{messageName}_EMPTY";

    public static string MessageEndInvalid(string messageName) => $"MSG_{messageName}_END_INVALID";

    public static string MessageMissingMandatory(string messageName) => $"MSG_{messageName}_MISSING_MANDATORY";

    public static string MessageDuplicate(string messageName) => $"MSG_{messageName}_DUPLICATE";
}

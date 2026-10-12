namespace IataText.Parser.Contracts;

public static class ErrorCodes
{
    public static readonly string MISSING_REQUIRED_ELEMENT          = "MSG_01";
    public static readonly string NO_MSG_REGISTERED_HANDLER         = "MSG_HND_01";
    public static readonly string ERROR_CODE_MSG_LEN_NOT_CORRECT    = "1001";
    public static readonly string ERROR_CODE_VALUE_NOT_CORRECT      = "1002";
    public static readonly string BAGGAGE_SOURCE_INDICATOR_NOT_CORRECT = "V_01";
    public static readonly string ACK_NAK_VALUE_NOT_CORRECT         = "A_01";
    public static readonly string BAG_STATUS_VALUE_NOT_CORRECT      = "B_01";
    public static readonly string PASSENGER_STATUS_VALUE_NOT_CORRECT = "S_01";
    public static readonly string PASSENGER_PROFIL_STATUS_VALUE_NOT_CORRECT = "S_02";
    public static readonly string BAGTAG_STATUS_VALUE_NOT_CORRECT = "S_03";
    public static readonly string OUTBOUND_FLIGHT_NOT_CORRECT       = "F_01";
}

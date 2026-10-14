namespace Aero.Scanner.Api.Contracts;

public class BaggageConsts
{
    public readonly static char FLIGHT_NUMBER_PAD_CHAR = '0';
    public readonly static string LOCAL_SOURCE_INDICATOR = "LOCAL_SOURCE_INDICATOR";
    public readonly static string AUTHORITY_LOAD_FLAG = "AUTHORITY_LOAD_FLAG";
    public static readonly string[] CHANGE_OF_STATUS = ["ACK", "NAK"];
    public static readonly string[] BAG_STATUS_CODE_ARRAY = ["NAL", "OFF", "UNS", "OND", "ONA"];
    public static readonly string[] AUTHORITY_LOAD_ARRAY = ["Y", "N"];
    public static readonly string[] PASSENGER_STATUS_ARRAY = ["B", "C", "N", "S"];
    public static readonly char[] BAG_SOURCE_INDICATOR_ARRAY = ['L', 'T', 'X', 'R'];
    public static readonly string[] PIECE_WEIGHT_ARRAY = ["P", "L", "K"];
    public static readonly string[] SCREEN_INSTRUCTION_ARRAY = ["SEL", "NON"];
    public static readonly string[] SCREEN_RESULT_ARRAY = ["CLR", "REF", "UCL"];
    public static readonly string[] SCREEN_RESULT_REASON_ARRAY = ["D", "E", "C", "N", "T"];
    public static readonly string[] SECONDARY_CODE_ARRAY = ["G", "R", "S", "H"];
    public static readonly string[] TYPE_OF_BAGGAGE_ARRAY = ["M", "T", "S", "X"];
    public static readonly string[] UnitValues = ["CM", "IN"];
    public static readonly string[] ValidMonths = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
    public static readonly string[] SealedIndicatorValues = ["Y", "N"];
    public static readonly string[] YesNoValues = ["Y", "N"];
    public static readonly string[] BagTagStatusValues = ["I", "A"];
    public static readonly string[] ValidTimeSuffixes = ["L", "Z"];


}
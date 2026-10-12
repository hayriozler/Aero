using System;
using System.Collections.Generic;
using System.Text;

namespace IataText.Parser.Contracts;

public class Consts
{
    public readonly static char DOT = '.';
    public readonly static char FLIGHT_NUMBER_PAD_CHAR = '0';
    public static char[] NEW_LINE_SEPERATOR => Environment.NewLine == "\r\n" ? ['\r', '\n'] : ['\n'];
    public static char[] ELEMENT_SEPERATOR => ['/'];

    public const string BCM = "BCM"; // Baggage Control Message, it is the main message and other messages are related to it by using its identifier as secondary identifier
    public const string BAM = "BAM"; // Baggage Acknowledgement Message
    public const string END = "END"; // End of Message, it is used to identify the end of message and the type of message it is ending for is identified by using the secondary identifier
    public const string FCM = "FCM"; // Flight Close Message
    public const string FMM = "FMM"; // Final Match Message
    public const string BMM = "BMM"; // Baggage Manifest Message
    public const string BNS = "BNS"; // Baggage Not Seend Message
    public const string BPM = "BPM"; // Baggage Processed Message
    public const string BRQ = "BRQ"; // Baggage Request Message
    public const string BSM = "BSM"; // Baggage Source Message
    public const string CHG = "CHG"; // Baggage Source Change Message also can be used for deleting baggage
    public const string DEL = "DEL"; // Baggage Source Delete Message
    public const string BTM = "BTM"; // Baggage Transfer Message
    public const string BUM = "BUM"; // Baggage Unload Message
    public const string DBM = "DBM"; // Delete Baggage Message
    public const string FOM = "FOM"; // Flight Open Message

    public const string A = ".A/";
    public const string B = ".B/";
    public const string C = ".C/";
    public const string D = ".D/";
    public const string E = ".E/";
    public const string F = ".F/";
    public const string G = ".G/";
    public const string H = ".H/";
    public const string I = ".I/";
    public const string J = ".J/";
    public const string K = ".K/";
    public const string L = ".L/";
    public const string N = ".N/";
    public const string O = ".O/";
    public const string P = ".P/";
    public const string Q = ".Q/";
    public const string R = ".R/";
    public const string S = ".S/";
    public const string T = ".T/";
    public const string U = ".U/";
    public const string V = ".V/";
    public const string W = ".W/";
    public const string X = ".X/";
    public const string Y = ".Y/";

    public readonly static string LOCAL_SOURCE_INDICATOR = "LOCAL_SOURCE_INDICATOR";
    public readonly static string AUTHORITY_LOAD_FLAG = "AUTHORITY_LOAD_FLAG";
    public static readonly string[] CHANGE_OF_STATUS = ["ACK", "NAK"];
    /// <summary>
    /// Only these message types support multi-part (partitioned) messages per RP 1745.
    /// </summary>
    public static readonly HashSet<string> PartitionableIdentifiers = [BPM, BNS, BCM, BMM];

    public static readonly string[] ACK_NAK_ARRAY = ["ACK", "NAK"];

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDFInterface
{
    using System.ComponentModel;
    public enum QueryTypes : int
    {
        [Description("Quotes")]
        Quotes = 0,

        [Description("Portfolio Mgr")]
        Portfolio_Mgr = 1,

        [Description("Fundamentals")]
        Fundamentals = 2,

        [Description("Dynamic Quotes")]
        Dynamic_Quotes = 3,

        [Description("Charts")]
        Charts = 4,

        [Description("Change Since")]
        ChangeSince = 5,

        [Description("Pulse")]
        Pulse = 6,

    }

    public enum XMLTypes
    {
        [Description("Unknown")]
        Unknown = 0,

        [Description("XMLCharts")]
        XMLCharts = 1,

        [Description("marketPages")]
        marketPages = 2,

        [Description("bpPages")]
        bpPages = 3,

        [Description("marketStatistics")]
        marketStatistics = 4,

    }

    public enum RequestTypes
    {
        [Description("Quotes")]
        Quotes = 0,

        [Description("Charts")]
        Charts = 1,

        [Description("Change Since")]
        ChangeSince = 2,

        [Description("Pulse")]
        Pulse = 3,

        [Description("Winners and losers")]
        Winners = 4,

    }


    public class TDFconstants
    {
        // ITF Message Types
        public const byte SYNC = 0x02;
        public const ushort PROT_ID = 0x0300;  // 0x03 L.E.
        public const byte DATA_OFFSET = 0x0c; //             12
        public const byte LOGON_REQUEST = 0x4c; // 'L'       76
        public const byte LOGOFF_REQUEST = 0x43; // 'C'      67
        public const byte DATA_REQUEST = 0x51; // 'Q'        81
        public const byte LOGON_RESPONSE = 0x49; // 'I'      73
        public const byte LOGOFF_RESPONSE = 0x58; // 'X'     88
        public const byte DATA_RESPONSE = 0X5a; // 'Z'       90
        public const byte DYNAMIC_UPDATE = 0X55; //'U'       85
        public const byte DYNAMIC_CONTROL = 0X59; //'Y'      89
        public const byte KEEP_ALIVE_REQUEST = 0X4b; //'K'   75
        public const byte KEEP_ALIVE_RESPONSE = 0x6b; // 'k' 107

        public const byte MSG_TERMINATOR = 0x00;

        // Response Type Data Response
        public const byte SUCCCESSFUL_LOGON_LOGOFF = 0x52; // 'R'  82
        public const byte ERROR_LOGON_LOGOFF = 0x45; //'E'      69
        public const ushort OPEN_FID_RESPONSE = 0x17; //        23
        public const ushort CATALOGER_RESPONSE = 0x19; //       25
        public const ushort SUBSCRIPTION_RESPONSE = 0x1b; //    27
        public const ushort UNSUBSCRIPTION_RESPONSE = 0x1c; //  28
        public const ushort XML_RESPONSE = 0x28; //             40
        public const ushort XML_CHART_RESPONSE = 0x35; //       53

    }

    public class TDFGlobals
    {
        public static List<symbolData> symbols = new List<symbolData>();
        public static List<byte> TRdata = new List<byte>();
        public static int dataLeft;
        public static bool loggedIn = false;
        public static string logResp = "";
        public static List<string> catStr = new List<string>();
        public static bool pageDataFlag = false;
        public static AsyncClientSocket.ClientSocket TRClientSocket;
        public static bool TRConnected = false;
        public static int ServerID = 0;
        public static bool dynamic = false;

        
        public static field_Info[] field_Info_Table = new field_Info[0x10000];
        public static List<string> starredFields = new List<string>();
        public static List<fin_Data> financialResults = new List<fin_Data>();
        public static Int32[,] CatalogData = new int[150, 60];
        public static Int16 numCat { get; set; }

        public static bool showAllFields { get; set; }
        public static List<string> Dow30symbols = new List<string>();

        public List<string> holidays = new List<string>();

        public static bool liveUpdateFlag = false;
        public static bool[] seqIdsUsed = new bool[65000];
        public static bool[] seqIdsrecd = new bool[65000];
        public static uint seqStrt;
        public static int requestId = 0;
        public DateTime serverReset;
        public static TimeSpan marketOpen = new TimeSpan(9, 30, 00); //9:30 am
        public static TimeSpan marketClose = new TimeSpan(16, 00, 0); //4:00 pm 
        public static bool marketOpenStatus;




    }

}

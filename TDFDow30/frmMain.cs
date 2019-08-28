using System;
using System.Collections.Generic;
using log4net.Appender;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AsyncClientSocket;
using TDFInterface;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;


namespace TDFDow30
{
    
    public partial class frmMain : Form, IAppender
    {
        #region Globals
        DateTime referenceTime = DateTime.MaxValue;
        DateTime dataReceivedTime = DateTime.MaxValue;
        public XmlDocument xmlResponse = new XmlDocument();
        BindingList<Dow30Database.Dow30DB.Dow30symbolData> Dow30Data = new BindingList<Dow30Database.Dow30DB.Dow30symbolData>();
        Stopwatch stopWatch = new Stopwatch();
        TimeSpan ts;


        itf_Header stdHeadr = new itf_Header()
        {
            sync = TDFconstants.SYNC,
            msgType = TDFconstants.LOGON_REQUEST,
            protId = TDFconstants.PROT_ID,
            seqId = 0,
            sessionId = 0xffff,
            msgSize = 0,
            dataOffset = TDFconstants.DATA_OFFSET
        };

        // Default Login Info
        public string IPAddress = "";
        public string Port = "";
        public string UserName = "";
        public string PW = "";

        //public string logResp = "";
        public ushort session_ID = 0xffff;


        //public Int32[,] CatalogData = new int[150, 60];
        public string msgStr = "";
        public string XMLStr = "";

        public string quot = "\"";
        public int rCnt = 0;
        public Int64 bytesReceived = 0;
        public bool moreData = false;
        public bool traversal = true;
        public string unsubscribeSymbol = "";
        public int numLog = 0;

        public itf_Parser_Return_Message tmpMessage = new itf_Parser_Return_Message();
        public List<byte> TRdata = new List<byte>();
        public List<string> catStr = new List<string>();
        public List<itf_Parser_Return_Message> recMessages = new List<itf_Parser_Return_Message>();

        public bool showCatalog = false;
        public bool showFIT = false;
        public bool pageDataFlag = false;
        public string FITstr = "";

        public string lastQuery = null;


        public Chart_Data ch = new Chart_Data();
        public Chart_Info chartInfo = new Chart_Info();
        public List<Chart_Data> charts = new List<Chart_Data>();
        public int nchart = 0;
        public string statusStr = "";
        public pageData marketPage = new pageData();
        public Stopwatch sw;
        public string dbConn = "";
        public string dbTableName = "";
        public string dbChartTableName = "";
        public string symbolListStr = "";
        public DateTime connectTime;
        public DateTime disconnectTime;
        public DateTime refTime;
        public bool resetting = false;
        public bool resetComplete = false;
        public bool loggedIn = false;
        public bool dynamic = false;
        public List<string> messages = new List<string>();
        public string zipperFilePath;
        public bool debugMode = false;
        public bool timerFlag = false;
        public bool resetFlag = false;
        public bool zipperFlag = false;
        public string spName = "";
        public DateTime timerEmailSent = DateTime.Now.AddDays(-1);
        public DateTime zipperEmailSent = DateTime.Now.AddDays(-1);
        public bool marketIsOpen = false;
        public float dowValue;
        public float nasdaqValue;
        public float spxValue;
        public Int16 chartCnt = 0;
        public Int16 chartInterval = 1;
        public bool updateZipperFile = false;
        public bool updateChartData = false;
        public string spUpdateChart = "";
        
        public byte mt = 0;
        public ushort msgSize = 0;
        public int dataLeft = 0;

        public bool dataReset = false;
        //public DateTime nextServerReset;
        //public DateTime nextDailyReset;
        

        TimeSpan marketOpen = new TimeSpan(9, 29, 58); //9:30 am
        TimeSpan marketClose = new TimeSpan(16, 10, 0); //4:06 pm  somtimes data is updated a bit after market close


        public class X20ChartData
        {
            public float dow { get; set; }
            public float nasdaq { get; set; }
            public float spx { get; set; }
        }
        public X20ChartData X20_chartData = new X20ChartData();


        public class XMLUpdateEventArgs : EventArgs
        {
            public string XML { get; set; }
        }

        public List<Dow30Database.Dow30DB.MarketHolidays> marketHolidays = new List<Dow30Database.Dow30DB.MarketHolidays>();

        
        #endregion

        #region Collection, bindilist & variable definitions

        /// <summary>
        /// Define classes for collections and logic
        /// </summary>

        // Declare TCP client sockets for Thomson Reuters communications
        //public AsyncClientSocket.ClientSocket TRClientSocket;
        //bool TRConnected = false;


        #endregion

        #region Logger instantiation - uses reflection to get module name
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Logging & status setup
        // This method used to implement IAppender interface from log4net; to support custom appends to status strip
        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            // Set text on status bar only if logging level is DEBUG or ERROR
            if (loggingEvent.Level.Name == "ERROR")
            {
                //toolStripStatusLabel.BackColor = System.Drawing.Color.Red;
                //toolStripStatusLabel.Text = String.Format("Error Logging Message: {0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
            }
            else
            {
                //toolStripStatusLabel.BackColor = System.Drawing.Color.SpringGreen;
                //toolStripStatusLabel.Text = String.Format("Status Logging Message: {0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
            }
        }

        // Handler to clear status bar message and reset color
        private void resetStatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //toolStripStatusLabel.BackColor = System.Drawing.Color.SpringGreen;
            //toolStripStatusLabel.Text = "Status Logging Message: Statusbar reset @" + DateTime.Now.ToString();
        }
        #endregion

        public frmMain()
        {
            InitializeComponent();
            TDFConnections.ReconnectTimerInit();
        }

        // public event EventHandler<SymbolUpdateEventArgs> SymbolDataUpdated;

        //public event EventHandler<ChartLiveUpdateEventArgs> ChartDataUpdated;
        //public event EventHandler<XMLUpdateEventArgs> XMLDataUpdated;
        //public event EventHandler<ChartClosedEventArgs> ChartClosed;

        //protected virtual void OnSymbolDataUpdated(SymbolUpdateEventArgs e)
        //{

        //    EventHandler<SymbolUpdateEventArgs> evntH = SymbolDataUpdated;
        //    if (evntH != null)
        //        evntH(this, e);


        //    //SymbolDataUpdated?.Invoke(this, e);
        //}
        /*
        protected virtual void OnChartDataUpdated(ChartLiveUpdateEventArgs e)
        {

            EventHandler<ChartLiveUpdateEventArgs> evntH = ChartDataUpdated;
            if (evntH != null)
                evntH(this, e);

            //ChartDataUpdated?.Invoke(this, e);
        }

        
        protected virtual void OnXMLDataUpdated(XMLUpdateEventArgs e)
        {

            EventHandler<XMLUpdateEventArgs> evntH = XMLDataUpdated;
            if (evntH != null)
                evntH(this, e);

        }
        */


        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                // Read in config settings
                // Display host name and IP address
                string hostIpAddress = HostIPNameFunctions.GetLocalIPAddress();
                string hostName = HostIPNameFunctions.GetHostName(hostIpAddress);
                lblIpAddress.Text = hostIpAddress;
                lblHostName.Text = hostName;



                IPAddress = Properties.Settings.Default.TDF_IPAddress;
                Port = Properties.Settings.Default.TDF_Port;
                UserName = Properties.Settings.Default.TDF_UserName;
                PW = Properties.Settings.Default.TDF_PW;

                dbConn = Properties.Settings.Default.dbConn;
                dbTableName = Properties.Settings.Default.dbTableName;
                dbChartTableName = Properties.Settings.Default.chartTableName;
                spName = Properties.Settings.Default.spUpdate;
                spUpdateChart = Properties.Settings.Default.spUpdateChart;
                dynamic = Properties.Settings.Default.Dynamic;
                zipperFilePath = Properties.Settings.Default.ZipperFilePath;
                debugMode = Properties.Settings.Default.DebugMode;
                updateZipperFile = Properties.Settings.Default.updateZipperFile;
                updateChartData = Properties.Settings.Default.updateChartData;
                TDFGlobals.ServerID = Properties.Settings.Default.TDFServer_ID;

                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(TDFGlobals.ServerID);
                IPAddress = sr.IPAddress;
                UserName = sr.UserId;
                PW = sr.PW;
                Port = sr.Port.ToString();

                IPTextBox.Text = IPAddress;
                PortTextBox.Text = Port;
                UserTextBox.Text = UserName;
                PWTextBox.Text = PW;
                ServerTextBox.Text = TDFGlobals.ServerID.ToString();

                TDFGlobals.showAllFields = false;

                TDFProcessingFunctions.InitializeSymbolFields();
                ServerResetLabel.Text = $"Next Server Reset: {TDFConnections.nextServerReset}";
                DailyResetLabel.Text = $"Next Daily Reset: {TDFConnections.nextDailyReset}";

                
                string cmd = $"SELECT * FROM MarketHolidays";
                marketHolidays = Dow30Database.Dow30DB.GetHolidays(cmd, dbConn);

                marketIsOpen = MarketOpenStatus();
                if (marketIsOpen)
                    dataReset = false;

                // Set version number
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                this.Text = String.Format("TDF Dow30 Application  Version {0}", version);

                // Log application start
                log.Debug($"\r\n\r\n*********** Starting TDFDow30 v{version} **********\r\n");

                chartCnt = (Int16)(chartInterval - 3);
                TDFConnections.ConnectToTDF(TDFGlobals.ServerID);
                if (TDFGlobals.TRConnected == true)
                {
                    pictureBox2.Visible = true;
                }
                lblLogResp.Text = TDFGlobals.logResp;

                InitializeDow30Data();
                TODTimer.Enabled = true;
                //ResetTimer.Enabled = true;

                //TDFConnections.nextDailyReset = DateTime.Now.AddMinutes(5);

            }
            catch (Exception ex)
            {
                // Log error
                log.Error("frmMain Exception occurred during main form load: " + ex.Message);
                //log.Debug("frmMain Exception occurred during main form load", ex);
            }
            
        }

        public void InitializeDow30Data()
        {
            // get data from db table to get symbol list
            string connection = $"SELECT * FROM {dbTableName}";
            Dow30Data = Dow30Database.Dow30DB.GetSymbolDataCollection(connection, dbConn);
            symbolDataGrid.DataSource = Dow30Data;

            log.Debug("Initializing symbols...");
            System.Threading.Thread.Sleep(1000);

            // create symbol list and set up symbols collection
            bool first = true;
            symbolListStr = "";
            uint ui = 0;
            TDFGlobals.Dow30symbols.Clear();
            foreach (Dow30Database.Dow30DB.Dow30symbolData sd in Dow30Data)
            {
                TDFGlobals.Dow30symbols.Add(sd.SubscribeSymbol);
                if (first == false)
                {
                    symbolListStr += ", " + sd.SubscribeSymbol;
                }
                else
                {
                    symbolListStr += sd.SubscribeSymbol;
                    first = false;
                }

                symbolData sd1 = new symbolData();
                if (dynamic)
                    sd1.queryType = (int)QueryTypes.Dynamic_Quotes;
                else
                    sd1.queryType = (int)QueryTypes.Portfolio_Mgr;
                sd1.queryStr = "";
                sd1.symbol = sd.SubscribeSymbol;
                sd1.company_Name = sd.DisplayName.ToUpper();
                sd1.seqId = 5;
                sd1.updated = DateTime.Now;
                TDFGlobals.symbols.Add(sd1);
                if (dynamic)
                {
                    ui++;
                    IssueDynamicSubscriptionQuery(sd.SubscribeSymbol, ui);
                    Thread.Sleep(50);
                }

            }
            //label1.Text = symbolListStr;

            // start data collection
            log.Debug("Symbols initialized...");
            Thread.Sleep(200);

            timer1.Enabled = true;

        }


        private void ConnectButton_Click(object sender, EventArgs e)
        {
            TDFConnections.ConnectToTDF(TDFGlobals.ServerID);
        }

        /*
        public void ConnectToTDF()
        {
            // Build Logon Message
            string queryStr = "LOGON USERNAME" + "=\"" + UserTextBox.Text + "\" PASSWORD=\"" +
                PWTextBox.Text + "\"";


            

            // Instantiate and setup the client sockets
            // Establish the remote endpoints for the sockets
            System.Net.IPAddress TRIpAddress = System.Net.IPAddress.Parse(IPAddress);
            TDFGlobals.TRClientSocket = new ClientSocket(TRIpAddress, Convert.ToInt32(Port));

            // Initialize event handlers for the sockets
            TDFGlobals.TRClientSocket.DataReceived += TDFProcessingFunctions.TRDataReceived;
            TDFGlobals.TRClientSocket.ConnectionStatusChanged += TRConnectionStatusChanged;

            // Connect to the TRClientSocket; call-backs for connection status will indicate status of client sockets
            TDFGlobals.TRClientSocket.AutoReconnect = true;
            TDFGlobals.TRClientSocket.Connect();

            System.Threading.Thread.Sleep(1000);
            if (TDFGlobals.TRConnected == true)
            {
                pictureBox2.Visible = true;
            }

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.LOGON_REQUEST, 0);
            TDFConnections.TRSendCommand(outputbuf);

            int n = 0;
            while (loggedIn == false && n < 50)
            {
                n++;
                System.Threading.Thread.Sleep(100);

            }
            TDFConnections.TRSendCommand(outputbuf);
            lblLogResp.Text = TDFGlobals.logResp;
            TDFProcessingFunctions.GetCataloger();
            //label7.Text = "Num Catalogs: " + numCat.ToString();

            TDFProcessingFunctions.GetFieldInfoTable();
            System.Threading.Thread.Sleep(1000);
            Int32 cnt = 0;

            for (int i = 0; i < TDFGlobals.field_Info_Table.Length; i++)
            {
                if (TDFGlobals.field_Info_Table[i].fieldId > 0)
                    cnt++;

            }

            label6.Text = "Number of Fields: " + cnt.ToString();
            label7.Text = "Num Catalogs: " + TDFGlobals.numCat.ToString();

            
            // get data from db table to get symbol list
            string connection = $"SELECT * FROM {dbTableName}";
            Dow30Data = Dow30Database.Dow30DB.GetSymbolDataCollection(connection, dbConn);
            symbolDataGrid.DataSource = Dow30Data;

            System.Threading.Thread.Sleep(1000);

            // create symbol list and set up symbols collection
            bool first = true;
            symbolListStr = "";
            uint ui = 0;
            foreach (Dow30Database.Dow30DB.Dow30symbolData sd in Dow30Data)
            {
                TDFGlobals.Dow30symbols.Add(sd.SubscribeSymbol);
                if (first == false)
                {
                    symbolListStr += ", " + sd.SubscribeSymbol;
                }
                else
                {
                    symbolListStr += sd.SubscribeSymbol;
                    first = false;
                }

                symbolData sd1 = new symbolData();
                if (dynamic)
                    sd1.queryType = (int)QueryTypes.Dynamic_Quotes;
                else
                    sd1.queryType = (int)QueryTypes.Portfolio_Mgr;
                sd1.queryStr = "";
                sd1.symbol = sd.SubscribeSymbol;
                sd1.company_Name = sd.DisplayName.ToUpper();
                sd1.seqId = 5;
                sd1.updated = DateTime.Now;
                TDFGlobals.symbols.Add(sd1);
                if (dynamic)
                {
                    ui++;
                    IssueDynamicSubscriptionQuery(sd.SubscribeSymbol, ui);
                    Thread.Sleep(50);
                }

            }
            //label1.Text = symbolListStr;

            // start data collection
            timer1.Enabled = true;
            
        }
        */ 
        public void IssueDynamicSubscriptionQuery(string symbolStr, uint seq)
        {
            string fieldList = "trdPrc, netChg, pcntChg"; 
            //string query = "SELECT " + fieldList + " FROM DYNAMIC_QUOTES WHERE usrSymbol= \"" + symbolStr + "\"";
            string query = $"SELECT {fieldList} FROM DYNAMIC_QUOTES WHERE usrSymbol= {quot}{symbolStr}{quot}";
            if (TDFGlobals.TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, seq);

                //TRSendCommand(outputbuf);
                TDFConnections.TRSendCommand(outputbuf);

                listBox1.Items.Add(query);

            }
        }

        #region Socket Handlers
        // Handler for data received back from TRclientsocket
        /*
        private void TRDataReceived(ClientSocket sender, byte[] data)
        {

            TDFDataReceived(sender, data);
        }

        private void TDFDataReceived(ClientSocket sender, byte[] data)
        {
            try
            {
                // receive the data and determine the type
                int bufLen = sender.bufLen;
                rCnt++;
                bytesReceived += bufLen;
                byte[] rData = new byte[bufLen];
                Array.Copy(data, 0, rData, 0, bufLen);
                TRdata.AddRange(rData);
                //TRdata.AddRange(data);
                bool waitForData = false;
                bool dynamicFlag = false;
                int len = 0;
                mt = 0;
                msgSize = 0;

                dataLeft = TRdata.Count;
                dataReceivedTime = DateTime.Now;

                TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();

                while (dataLeft >= 23 && waitForData == false)
                {
                    ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                    itf_Parser_Return_Message TRmessage = new itf_Parser_Return_Message();
                    itf_Parser_Update_Message TRupdateMessage = new itf_Parser_Update_Message();
                    itf_Control_Message TRControlMessage = new itf_Control_Message();

                    mt = itfHeaderAccess.GetMsgType(TRdata.ToArray());
                    msgSize = itfHeaderAccess.GetMsgSize(TRdata.ToArray());

                    if (msgSize <= dataLeft)
                    {
                        if (mt == TDFconstants.DYNAMIC_UPDATE)
                        {
                            try
                            {
                                TRupdateMessage = itfHeaderAccess.ParseItfUpdateMessage(TRdata.ToArray());
                                if (msgSize <= TRupdateMessage.totalMessageSize)
                                    TDFproc.ProcessFinancialUpdateData(TRupdateMessage);
                                //Task.Run(() => TDFproc.ProcessFinancialUpdateData(TRupdateMessage));
                                if (msgSize + 1 >= TRdata.Count)
                                    len = TRdata.Count;
                                else
                                    len = msgSize + 1;
                                //TRdata.RemoveRange(0, msgSize + 1);
                                TRdata.RemoveRange(0, len);
                                dataLeft = TRdata.Count;
                                dynamicFlag = true;
                            }
                            catch (Exception ex)
                            {
                                log.Error($"Dynamic Update error: {ex}");
                            }
                        }
                        else if (mt == TDFconstants.DYNAMIC_CONTROL)
                        {

                            TRControlMessage = itfHeaderAccess.ParseItfControlMessage(TRdata.ToArray());
                            log.Info($"Control Message Code: {TRControlMessage.control_Message_Header.messageCode}");
                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                            dataLeft = TRdata.Count;
                        }
                        else if (mt == TDFconstants.LOGOFF_RESPONSE)
                        {
                            TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                            TDFGlobals.logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            messages.Add("Logoff at " + DateTime.Now.ToString());
                            log.Info(TDFGlobals.logResp);
                            TRdata.Clear();
                            dataLeft = TRdata.Count;
                            loggedIn = false;

                            switch (TRmessage.data_Header.respType)
                            {
                                case TDFconstants.SUCCCESSFUL_LOGON_LOGOFF:
                                    log.Info("Logoff " + TDFGlobals.logResp);
                                    break;

                                case TDFconstants.ERROR_LOGON_LOGOFF:
                                    log.Info("Logoff Error " + TDFGlobals.logResp);
                                    break;
                            }
                        }
                        else if (mt == TDFconstants.LOGON_RESPONSE)
                        {
                            TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                            TDFGlobals.logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            messages.Add("Logon at " + DateTime.Now.ToString());
                            TRdata.Clear();
                            dataLeft = TRdata.Count;

                            switch (TRmessage.data_Header.respType)
                            {
                                case TDFconstants.SUCCCESSFUL_LOGON_LOGOFF:
                                    // get and save session ID
                                    stdHeadr.sessionId = TRmessage.itf_Header.sessionId;
                                    log.Info("Logon at " + DateTime.Now.ToString());
                                    log.Info(TDFGlobals.logResp);
                                    loggedIn = true;
                                    break;

                                case TDFconstants.ERROR_LOGON_LOGOFF:
                                    log.Info("Logon Error " + TDFGlobals.logResp);
                                    loggedIn = false;
                                    break;
                            }

                        }
                        else if (mt == TDFconstants.KEEP_ALIVE_REQUEST)
                        {
                            try
                            {
                                string ka = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                statusStr = "Keep Alive at " + DateTime.Now.ToString() + " 1 " + ka;
                                messages.Add(statusStr);
                                log.Info(statusStr);

                                ProcessKeepAliveRequest(TRmessage);
                                if (TRdata.Count >= msgSize + 1)
                                    TRdata.RemoveRange(0, msgSize + 1);
                                else
                                    TRdata.RemoveRange(0, TRdata.Count);

                                dataLeft = TRdata.Count;
                            }
                            catch (Exception ex)
                            {
                                log.Error($"KEEP ALIVE REQUEST error: {ex}");
                            }
                        }
                        else if (mt == TDFconstants.DATA_RESPONSE)
                        {
                            try
                            {
                                TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                                switch (TRmessage.data_Header.respType)
                                {

                                    case TDFconstants.CATALOGER_RESPONSE:
                                        catStr = TDFproc.ProcessCataloger(TRmessage.Message.ToArray());
                                        TRdata.Clear();
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.OPEN_FID_RESPONSE:
                                        if (TRmessage.itf_Header.seqId == 98)
                                        {
                                            TDFproc.ProcessFieldInfoTable(TRmessage);
                                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                            dataLeft = TRdata.Count;
                                        }
                                        else
                                        {
                                            TDFproc.ProcessFinancialData(TRmessage);
                                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                            dataLeft = TRdata.Count;
                                            WatchdogTimer.Enabled = false;
                                        }
                                        break;

                                    case TDFconstants.SUBSCRIPTION_RESPONSE:
                                        TDFproc.ProcessFinancialData(TRmessage);
                                        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.UNSUBSCRIPTION_RESPONSE:
                                        TRdata.RemoveRange(0, msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.XML_RESPONSE:
                                        XMLStr = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                        MemoryStream ms = new MemoryStream(TRmessage.Message.ToArray());
                                        xmlResponse.Load(ms);
                                        break;

                                    case TDFconstants.XML_CHART_RESPONSE:
                                        XMLStr += System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                        PreProXML xmlData = TDFproc.GetXmlType(TRmessage);

                                        switch (xmlData.xmlCode)
                                        {
                                            case XMLTypes.XMLCharts:
                                                Chart_Data chart1Data = new Chart_Data();
                                                chart1Data = TDFproc.ProcessXMLChartData(xmlData);
                                                charts.Add(chart1Data);
                                                sw.Stop();
                                                break;

                                            case XMLTypes.marketPages:
                                                marketPage = TDFproc.ProcessMarketPages(xmlData);
                                                pageDataFlag = true;
                                                break;

                                            case XMLTypes.bpPages:
                                                marketPage = TDFproc.ProcessBusinessPages(xmlData);
                                                pageDataFlag = true;
                                                break;
                                        }

                                        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                    case TDFconstants.KEEP_ALIVE_REQUEST:
                                        string ka = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                                        statusStr = "Keep Alive at " + DateTime.Now.ToString() + " 2 " + ka;
                                        TDFProcessingFunctions.ProcessKeepAliveRequest(TRmessage);
                                        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize);
                                        dataLeft = TRdata.Count;
                                        statusStr = "Keep Alive at " + DateTime.Now.ToString();
                                        messages.Add(statusStr);
                                        break;

                                    default:
                                        statusStr = "Message type " + TRmessage.itf_Header.msgType.ToString() +
                                            "  Message Response " + TRmessage.data_Header.respType.ToString() + "  " + DateTime.Now.ToString();

                                        log.Error(statusStr);

                                        TRdata.RemoveRange(0, msgSize + 1);
                                        dataLeft = TRdata.Count;
                                        break;

                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error($"DATA_RESPONSE error: {ex}");
                            }
                        }
                        else
                        {
                            if (TRdata[0] == 2)
                                TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                            else
                            {
                                log.Error("--- Sync byte not found!");


                                
                                //TRdata.Clear();
                                //dataLeft = TRdata.Count;

                                //log.Debug("--- Receive buffer cleared!");
                                



                                int n = 0;
                                while (TRdata[0] != 2 && TRdata.Count > 0)
                                {
                                    TRdata.RemoveRange(0, 1);
                                    n++;
                                }

                                log.Debug($"--- {n} Bytes removed!");


                                //if (TRdata.Count > 0)
                                //TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                                //return;
                            }
                            
                            //if (TRmessage.itf_Header.msgType == TDFconstants.KEEP_ALIVE_REQUEST)
                            //{
                            //    cmdResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            //    //TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();
                            //    TDFproc.ProcessKeepAliveRequest(TRmessage);
                            //    int TRdataLen = TRdata.Count;
                            //    if (TRmessage.itf_Header.msgSize + 1 > TRdataLen)
                            //        TRdata.RemoveRange(0, TRdataLen);
                            //    else
                            //        TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                            //    dataLeft = TRdata.Count;
                            //}
                            
                        }
                    }
                    else
                        waitForData = true;

                }

                if (dynamicFlag)
                {
                    dynamicFlag = false;
                    //Task.Run(() => UpdateDynamicSymbols());
                    //UpdateDynamicSymbols();
                }

            }
            catch (Exception ex)
            {
                log.Error($"TRDataReceived error - {ex}");
            }
        }

        public void ProcessKeepAliveRequest(itf_Parser_Return_Message TRmess)
        {
            try
            {
                // Build Logon Message
                string queryStr = System.Text.Encoding.Default.GetString(TRmess.Message.ToArray());

                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.KEEP_ALIVE_RESPONSE, 0);
                TDFConnections.TRSendCommand(outputbuf);
                //TRSendCommand(outputbuf);
                //sendBuf(outputbuf);
            }
            catch (Exception ex)
            {
                log.Error($"Process KEEP ALIVE REQUEST error: {ex}");
            }
        }



        /*
    private void TRDataReceived(ClientSocket sender, byte[] data)
    {
        // receive the data and determine the type
        int bufLen = sender.bufLen;
        rCnt++;
        bytesReceived = bytesReceived + bufLen;
        byte[] rData = new byte[bufLen];
        Array.Copy(data, 0, rData, 0, bufLen);
        TRdata.AddRange(rData);
        bool waitForData = false;
        byte mt = 0;
        ushort msgSize = 0;

        dataReceivedTime = DateTime.Now;

        ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
        itf_Parser_Return_Message TRmessage = new itf_Parser_Return_Message();
        itf_Parser_Update_Message TRupdateMessage = new itf_Parser_Update_Message();
        TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();
        TDFProcessingFunctions.sendBuf += new SendBuf(TRSendCommand);

        int dataLeft = TRdata.Count;

        while (dataLeft > 0 && waitForData == false)
        {
            mt = itfHeaderAccess.GetMsgType(TRdata.ToArray());
            msgSize = itfHeaderAccess.GetMsgSize(TRdata.ToArray());

            //TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
            //if (TRmessage.itf_Header.msgSize <= dataLeft)
            if (msgSize <= dataLeft)
            {

                if (mt == TDFconstants.DYNAMIC_UPDATE)
                {
                    TRupdateMessage = itfHeaderAccess.ParseItfUpdateMessage(TRdata.ToArray());
                    TDFproc.ProcessFinancialUpdateData(TRupdateMessage);
                    TRdata.RemoveRange(0, msgSize + 1);
                    dataLeft = TRdata.Count;
                }
                else if (mt == TDFconstants.LOGOFF_RESPONSE)
                {
                    logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                    messages.Add("Logoff at " + DateTime.Now.ToString());
                    TRdata.Clear();
                    dataLeft = TRdata.Count;
                    loggedIn = false;

                }
                else if (mt == TDFconstants.KEEP_ALIVE_REQUEST)
                {
                    string ka = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                    statusStr = "Keep Alive at " + DateTime.Now.ToString() + " 1 " + ka;
                    messages.Add(statusStr);

                    TDFProcessingFunctions.ProcessKeepAliveRequest(TRmessage);
                    //TRdata.Clear();
                    if (TRdata.Count >= msgSize + 1)
                        TRdata.RemoveRange(0, msgSize + 1);
                    else
                        TRdata.RemoveRange(0, TRdata.Count);

                    dataLeft = TRdata.Count;

                }
                else
                {
                    if (TRdata[0] == 2)
                        TRmessage = itfHeaderAccess.ParseItfMessage(TRdata.ToArray());
                    else
                        return;
                    if (TRmessage.itf_Header.msgType == TDFconstants.KEEP_ALIVE_REQUEST)
                    {
                        logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                        TDFProcessingFunctions.ProcessKeepAliveRequest(TRmessage);
                        int TRdataLen = TRdata.Count;
                        if (TRmessage.itf_Header.msgSize + 1 > TRdataLen)
                            TRdata.RemoveRange(0, TRdataLen);
                        else
                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                        dataLeft = TRdata.Count;
                    }



                    switch (TRmessage.data_Header.respType)
                    {
                        case TDFconstants.SUCCCESSFUL_LOGON_LOGOFF:
                            numLog++;
                            logResp = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray()) + "    numLog: " + numLog.ToString();
                            messages.Add(logResp);
                            loggedIn = true;
                            // get and save session ID
                            stdHeadr.sessionId = TRmessage.itf_Header.sessionId;
                            rCnt = 0;
                            bytesReceived = 0;
                            TRdata.Clear();
                            dataLeft = TRdata.Count;
                            break;

                        case TDFconstants.CATALOGER_RESPONSE:
                            catStr = TDFproc.ProcessCataloger(TRmessage.Message.ToArray());
                            TRdata.Clear();
                            dataLeft = TRdata.Count;
                            break;

                        case TDFconstants.OPEN_FID_RESPONSE:
                            if (TRmessage.itf_Header.seqId == 98)
                            {
                                TDFproc.ProcessFieldInfoTable(TRmessage);
                                TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                dataLeft = TRdata.Count;
                            }
                            //else if (TRmessage.itf_Header.seqId < 10)
                            else
                            {
                                TDFproc.ProcessFinancialData(TRmessage);
                                TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                dataLeft = TRdata.Count;
                                stopWatch.Start();
                                // Get the elapsed time as a TimeSpan value.
                                ts = stopWatch.Elapsed;
                                stopWatch.Reset();
                                WatchdogTimer.Enabled = false;

                            }
                            break;

                        case TDFconstants.SUBSCRIPTION_RESPONSE:
                            //if (TRmessage.itf_Header.seqId == 101)
                            {
                                TDFproc.ProcessFinancialData(TRmessage);
                                TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                                dataLeft = TRdata.Count;
                            }
                            break;


                        case TDFconstants.UNSUBSCRIPTION_RESPONSE:
                            statusStr = "Unsubscribed at " + DateTime.Now.ToString();
                            TRdata.RemoveRange(0, msgSize + 1);
                            dataLeft = TRdata.Count;
                            break;


                        case TDFconstants.XML_RESPONSE:
                            XMLStr = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            MemoryStream ms = new MemoryStream(TRmessage.Message.ToArray());
                            xmlResponse.Load(ms);
                            XMLUpdateEventArgs xmldu = new XMLUpdateEventArgs();
                            xmldu.XML = XMLStr;
                            //XMLUpdateEventArgs xmlduc = new XMLUpdateEventArgs();
                            //xmlduc.XML = XMLStr;
                            //OnXMLDataUpdated(xmlduc);
                            //OnXMLDataUpdated(xmldu);
                            break;

                        case TDFconstants.XML_CHART_RESPONSE:
                            XMLStr += System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());

                            PreProXML xmlData = TDFproc.GetXmlType(TRmessage);


                            switch (xmlData.xmlCode)
                            {
                                case XMLTypes.XMLCharts:
                                    Chart_Data chart1Data = new Chart_Data();
                                    chart1Data = TDFproc.ProcessXMLChartData(xmlData);
                                    charts.Add(chart1Data);
                                    sw.Stop();
                                    break;

                                case XMLTypes.marketPages:
                                    marketPage = TDFproc.ProcessMarketPages(xmlData);
                                    pageDataFlag = true;
                                    break;

                                case XMLTypes.bpPages:
                                    marketPage = TDFproc.ProcessBusinessPages(xmlData);
                                    pageDataFlag = true;
                                    break;

                            }



                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize + 1);
                            dataLeft = TRdata.Count;
                            break;


                        case TDFconstants.KEEP_ALIVE_REQUEST:
                            string ka = System.Text.Encoding.Default.GetString(TRmessage.Message.ToArray());
                            statusStr = "Keep Alive at " + DateTime.Now.ToString() + " 2 " + ka;
                            TDFProcessingFunctions.ProcessKeepAliveRequest(TRmessage);
                            TRdata.RemoveRange(0, TRmessage.itf_Header.msgSize);
                            dataLeft = TRdata.Count;
                            statusStr = "Keep Alive at " + DateTime.Now.ToString();
                            messages.Add(statusStr);
                            break;

                        default:
                            statusStr = "Message type " + TRmessage.itf_Header.msgType.ToString() +
                                "  Message Response " + TRmessage.data_Header.respType.ToString() + "  " + DateTime.Now.ToString();

                            TRdata.RemoveRange(0, msgSize + 1);
                            dataLeft = TRdata.Count;
                            break;

                    }

                }
            }
            else
                waitForData = true;

        }

        // Log if debug mode
        //log.Debug("Command data received from source MSE: " + data);
        //TDFProcessingFunctions.SetSymbolData(fin_Data,)
    }


    */

        // Handler for source & destination MSE connection status change
        public void TRConnectionStatusChanged(ClientSocket sender, ClientSocket.ConnectionStatus status)
        {
            // Set status
            if (status == ClientSocket.ConnectionStatus.Connected)
            {
                TDFGlobals.TRConnected = true;
            }
            else
            {
                TDFGlobals.TRConnected = false;
            }
            if (debugMode)
                messages.Add("status: " + status.ToString());

            // Send to log - DEBUG ONLY
            log.Debug("TR Connection Status: " + status.ToString());
        }

        // Send a command to TDF
        /*
        public void TRSendCommand(byte[] outbuf)

        {
            try
            {
                // Send the data; terminiate with CRLF
                TRClientSocket.Send(outbuf);
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("TRSendCommand - Error occurred while trying to send data to TR client port: " + ex.Message);
            }
        }
        */
        #endregion;
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
        public void GetDow30Data()
        {
            try
            {
                //stopWatch.Start();
                string fieldList = "trdPrc, netChg, pcntChg";
                string query = $"SELECT {fieldList} FROM PORTFOLIO_MGR WHERE usrSymbol IN ({symbolListStr})";
                if (TDFGlobals.TRConnected)
                {
                    ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                    byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                    TDFConnections.TRSendCommand(outputbuf);
                    //TRSendCommand(outputbuf);
                    //WatchdogTimer.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("GetDow30Data - Error occurred getting the DOW30 data from TDF: " + ex.Message);
            }

        }

        public void GetYesterdaysClose()
        {
            string s = "";
            string sym = "";
            string oldSym = "";

            timer1.Enabled = false;
            Thread.Sleep(50);
            string query = "SELECT ycls FROM PORTFOLIO_MGR WHERE usrSymbol IN (.DJIA,.NCOMP,.SPX)";
            if (TDFGlobals.TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                TDFConnections.TRSendCommand(outputbuf);
                //TRSendCommand(outputbuf);
                //WatchdogTimer.Enabled = true;
            }
            Thread.Sleep(50);

            if (TDFGlobals.financialResults.Count > 0)
            {
                for (int i = 0; i < TDFGlobals.financialResults.Count; i++)
                {

                    int symbolIndex = TDFProcessingFunctions.GetSymbolIndx(TDFGlobals.financialResults[i].symbol);
                    sym = TDFGlobals.financialResults[i].symbol;
                    if (sym != oldSym && sym != null)
                    {
                        s = TDFGlobals.financialResults[i].symbolFull;

                        if (debugMode)
                        {
                            if (i == 0)
                                listBox1.Items.Add("----------");
                            if (dynamic == false)
                                listBox1.Items.Add(" ");
                            listBox1.Items.Add(s);

                        }

                        oldSym = sym;
                    }


                    if (TDFGlobals.financialResults.Count > 0)
                        TDFProcessingFunctions.SetSymbolData(TDFGlobals.financialResults, i, symbolIndex);

                }
                TDFGlobals.financialResults.Clear();

                UpdateAllSymbols(true);

                timer1.Enabled = true;


            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string s = "";
            string sym = "";
            string oldSym = "";

            try
            {

                if (dynamic == false && TDFGlobals.TRConnected && !resetting)
                {
                    GetDow30Data();
                    Thread.Sleep(50);
                }

                if (TDFGlobals.financialResults.Count > 0)
                {
                    WatchdogTimer.Enabled = false;
                    for (int i = 0; i < TDFGlobals.financialResults.Count; i++)
                    {

                        int symbolIndex = TDFProcessingFunctions.GetSymbolIndx(TDFGlobals.financialResults[i].symbol);
                        sym = TDFGlobals.financialResults[i].symbol;
                        if (sym != oldSym && sym != null)
                        {
                            s = TDFGlobals.financialResults[i].symbolFull;

                            if (debugMode)
                            {
                                if (i == 0)
                                    listBox1.Items.Add("----------");
                                if (dynamic == false)
                                    listBox1.Items.Add(" ");
                                listBox1.Items.Add(s);

                            }

                            oldSym = sym;
                        }

                        if (dynamic == false && debugMode == true)
                            DisplayResults(TDFGlobals.financialResults, i);

                        if (TDFGlobals.financialResults.Count > 0)
                            TDFProcessingFunctions.SetSymbolData(TDFGlobals.financialResults, i, symbolIndex);

                    }
                    TDFGlobals.financialResults.Clear();

                    UpdateAllSymbols(false);

                    string connection = $"SELECT * FROM {dbTableName}";
                    Dow30Data = Dow30Database.Dow30DB.GetSymbolDataCollection(connection, dbConn);
                    symbolDataGrid.DataSource = Dow30Data;
                    symbolDataGrid.ClearSelection();

                    foreach (DataGridViewRow row in symbolDataGrid.Rows)
                    {
                        if (Convert.ToSingle(row.Cells[5].Value) < 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.Red;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }
                        else if (Convert.ToSingle(row.Cells[5].Value) > 0)
                        {
                            row.DefaultCellStyle.BackColor = Color.Green;
                            row.DefaultCellStyle.ForeColor = Color.White;
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.White;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }

                    }
                    if (updateZipperFile)
                        UpdateZipperDataFile();
                    //label1.Text = $"Elapsed: {ts.TotalMilliseconds.ToString()} msec";
                }
            }
            catch (Exception ex)
            {
                log.Error($"Timer1 Error - {ex}");
            }
        }

        private void DisplayResults(List<fin_Data> f, int i)
        {
            string s = "";
            string tf = "";

            if (f[i].show && f[i].queryType != (int)(QueryTypes.Dynamic_Quotes))
            {
                switch (f[i].fieldDataType)
                {
                    case 1:
                        s = f[i].fieldName + ": " + f[i].iData.ToString();
                        listBox1.Items.Add(s);
                        break;
                    case 2:
                        s = f[i].fieldName + ": " + f[i].sData;
                        listBox1.Items.Add(s);
                        break;
                    case 3:
                        if (f[i].bData == 0)
                        {
                            tf = "False";
                        }
                        else
                        {
                            tf = "True";
                        }
                        s = f[i].fieldName + ": " + f[i].bData.ToString();
                        s = f[i].fieldName + ": " + tf;
                        listBox1.Items.Add(s);
                        break;
                    case 4:
                        s = f[i].fieldName + ": " + f[i].bData.ToString();
                        listBox1.Items.Add(s);
                        break;
                    case 5:
                        s = f[i].fieldName + ": " + f[i].fData.ToString();
                        listBox1.Items.Add(s);
                        break;
                    case 6:
                        s = f[i].fieldName + ": " + f[i].dData.ToString();
                        listBox1.Items.Add(s);
                        break;
                    case 9:
                        s = f[i].fieldName + ": " + f[i].sData;
                        listBox1.Items.Add(s);
                        break;
                    case 10:
                        s = f[i].fieldName + ": " + f[i].sData;
                        listBox1.Items.Add(s);
                        break;
                    case 13:
                        s = f[i].fieldName + ": " + f[i].hData.ToString();
                        listBox1.Items.Add(s);
                        break;
                    case 14:
                        s = f[i].fieldName + ": " + f[i].iData.ToString();
                        listBox1.Items.Add(s);
                        break;
                }
                //txtLog.Text += "\r\n" + s;
                listBox1.SelectedIndex = listBox1.Items.Count - 1;

            }

        }

        public void UpdateAllSymbols(bool ycls)
        {
            try
            {
                int nZeros = 0;
                for (int i = 0; i < TDFGlobals.symbols.Count; i++)
                {
                    symbolData sd = new symbolData();
                    sd = TDFGlobals.symbols[i];
                    if (marketIsOpen == false && sd.netChg == 0.0 && dataReset == false)
                        nZeros++;

                    if (Dow30Data.Count > 0 && Dow30Data[i].Change != sd.netChg && sd.trdPrc != 0)
                        UpdateDB(sd);

                    if (ycls)
                    {
                        if (sd.symbol == ".DJIA")
                            dowValue = sd.ycls;
                        if (sd.symbol == ".NCOMP")
                            nasdaqValue = sd.ycls;
                        if (sd.symbol == ".SPX")
                            spxValue = sd.ycls;

                    }
                    else
                    {
                        if (sd.symbol == ".DJIA")
                            dowValue = sd.trdPrc;
                        if (sd.symbol == ".NCOMP")
                            nasdaqValue = sd.trdPrc;
                        if (sd.symbol == ".SPX")
                            spxValue = sd.trdPrc;

                    }
                }
                if (nZeros > TDFGlobals.symbols.Count - 6 && dataReset == false)
                {
                    log.Debug($"Data reset at: {DateTime.Now}");
                    DataResetLabel.Text = $"Data reset at: {DateTime.Now}";
                    dataReset = true;
                }
            }
            catch (Exception ex)
            {
                log.Error($"UpdateAllSymbols - Error: {ex}");
            }
        }


        public void UpdateDB(symbolData sd)
        {
            //string cmdStr = "sp_UpdateSymbolData @Symbol, @Last, @Change, @PercentChange, @UpdateTime";
            string cmdStr = $"{spName} @Symbol, @Last, @Change, @PercentChange, @UpdateTime";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update Dow 30 Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                                try
                                {
                                    //Specify base command
                                    cmd.CommandText = cmdStr;

                                    cmd.Parameters.Add("@Symbol", SqlDbType.Text).Value = sd.symbol;
                                    cmd.Parameters.Add("@Last", SqlDbType.Float).Value = sd.trdPrc;
                                    cmd.Parameters.Add("@Change", SqlDbType.Float).Value = sd.netChg;
                                    cmd.Parameters.Add("@PercentChange", SqlDbType.Float).Value = sd.pcntChg;
                                    cmd.Parameters.Add("@UpdateTime", SqlDbType.DateTime).Value = DateTime.Now;
                                    
                                    sqlDataAdapter.SelectCommand = cmd;
                                    sqlDataAdapter.SelectCommand.Connection = connection;
                                    sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                                    // Execute stored proc to store top-level metadata
                                    sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                    //Attempt to commit the transaction
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    log.Error($"UpdateDB- SQL Command Exception occurred: {ex}");
                                }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateDB- SQL Connection Exception occurred: " + ex.Message);
                
            }
            
        }
        
        public void UpdateChartData(X20ChartData cd)
        {
            //string cmdStr = "sp_Insert_ChartData @Updated, @Dow, @NASDAQ, @SP";
            string cmdStr = spUpdateChart +  " @Updated, @Dow, @NASDAQ, @SP";

            //Save out the top-level metadata
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConn))
                {
                    connection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = connection.BeginTransaction("Update X20 Chart Data");

                            // Must assign both transaction object and connection 
                            // to Command object for a pending local transaction
                            cmd.Connection = connection;
                            cmd.Transaction = transaction;

                            try
                            {

                                Int64 UnixTimeInMSecUtc = GetCurrentUnixTimestampMillis();
                                string UnixTimeInMSecUtcStr = GetCurrentUnixTimestampMillisLocalTime().ToString();
                                //Specify base command
                                cmd.CommandText = cmdStr;

                                //cmd.Parameters.Add("@Updated", SqlDbType.BigInt).Value = UnixTimeInMSecUtc;
                                cmd.Parameters.Add("@Updated", SqlDbType.VarChar).Value = UnixTimeInMSecUtcStr;
                                cmd.Parameters.Add("@Dow", SqlDbType.Float).Value = cd.dow;
                                cmd.Parameters.Add("@NASDAQ", SqlDbType.Float).Value = cd.nasdaq;
                                cmd.Parameters.Add("@SP", SqlDbType.Float).Value = cd.spx;
                                
                                sqlDataAdapter.SelectCommand = cmd;
                                sqlDataAdapter.SelectCommand.Connection = connection;
                                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                                // Execute stored proc to store top-level metadata
                                sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                                //Attempt to commit the transaction
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                log.Error("Update X20 Chart Data- SQL Command Exception occurred: " + ex.Message);
                                log.Debug("Update X20 Chart Data- SQL Command Exception occurred", ex);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("UpdateData- SQL Connection Exception occurred: " + ex.Message);

            }

        }


        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }
        public static long GetCurrentUnixTimestampMillisLocalTime()
        {
            return (long)(DateTime.Now - UnixEpoch).TotalMilliseconds;
        }

        public void SendEmail(string msg)
        {
            MailMessage mail = new MailMessage("TDFDow30App@foxnews.com", "242 -GFX Engineering <GFXEngineering@FOXNEWS.COM>");
            //MailMessage mail = new MailMessage("TDFDow30App@foxnews.com", "alex.stivala@foxnews.com");

            SmtpClient mailClient = new SmtpClient();
            mailClient.Port = 25;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.UseDefaultCredentials = true;
            mailClient.Host = "10.232.16.121";
            mail.Subject = "TDFDow30 Error";
            //mail.Subject = "TDFDow30 Test Email";
            //mail.Body = "[" + DateTime.Now + "] " + Environment.NewLine + "The data monitor application has encountered a error" + Environment.NewLine + e.ToString();
            //mail.Body = "[" + DateTime.Now + "] " + Environment.NewLine + "This is a test message!" + Environment.NewLine;
            //mail.Body = "This is a greeting from the TDFDow30 application - just saying hello." + Environment.NewLine +
            //"No need to worry, no Fatal Exception Errors, no Warnings, just running smooooooth." + Environment.NewLine +
            //"Almost could be a Corona commercial.";
            mail.Body = msg;
            mailClient.Send(mail);
        }

        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dynamic)
                UnsubscribeAll();
            TDFConnections.Logoff();
            log.Debug("*****  TDFDow30 Closed *****");
        }
        
        private void TODTimer_Tick(object sender, EventArgs e)
        {
            timeOfDayLabel.Text = DateTime.Now.ToString("MMM d, yyyy -- h:mm:ss tt");

            if (TDFGlobals.TRConnected)
                pictureBox2.Visible = true;
            else
                pictureBox2.Visible = false;

            lblLogResp.Text = TDFGlobals.logResp;

            for (int i = 0; i < messages.Count; i++)
            {
                if (debugMode)
                    listBox1.Items.Add(messages[i]);

            }
            messages.Clear();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;

            
            DateTime timeNow = DateTime.Now;
            bool weekday;
            if (timeNow.DayOfWeek != DayOfWeek.Saturday && timeNow.DayOfWeek != DayOfWeek.Sunday)
                weekday = true;
            else
                weekday = false;

            bool todayIsHoliday = false;

            for (int i = 0; i < marketHolidays.Count; i++)
            {
                if (marketHolidays[i].holiDate == DateTime.Today)
                    todayIsHoliday = true;
            }

            //TimeSpan marketOpen = new TimeSpan(9, 29,58); //9:30 am
            //TimeSpan marketClose = new TimeSpan(16, 10, 0); //4:06 pm  somtimes data is updated a bit after market close
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime > marketOpen && currentTime < marketClose && weekday == true && todayIsHoliday == false)
            {

                if (marketIsOpen == false)
                {
                    string cmd = $"DELETE FROM " + dbChartTableName;
                    int numRows;
                    numRows = Dow30Database.Dow30DB.SQLExec(cmd, dbConn);
                    string s = $"Deleted {numRows} DB chart records.";
                    //listBox1.Items.Add(s);
                    log.Debug(s);
                    Thread.Sleep(50);
                    GetYesterdaysClose();
                    chartCnt = (short) (chartInterval - 2);

                }

                marketIsOpen = true;
                chartCnt++;

                if (chartCnt == chartInterval)
                {
                    chartCnt = 0;
                    X20_chartData.dow = dowValue;
                    X20_chartData.nasdaq = nasdaqValue;
                    X20_chartData.spx = spxValue;
                    if (updateChartData)
                        UpdateChartData(X20_chartData);

                }
            }
            else
                marketIsOpen = false;

            //if (timerFlag == true && DateTime.Now > refTime)
                //timerFlag = false;

            if (timerFlag == true && DateTime.Now > timerEmailSent.AddDays(1))
                timerFlag = false;

            // if server is scheduled for reset - Get next time for both resets
            if (DateTime.Now > TDFConnections.nextServerReset && !resetting)
            {
                resetting = true;
                timer1.Enabled = false;
                WatchdogTimer.Enabled = false;

                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(TDFGlobals.ServerID);
                TDFConnections.nextServerReset = TDFConnections.GetNextServerResetTime(sr);
                TDFConnections.nextDailyReset = TDFConnections.GetNextDailyResetTime(sr);
                ServerResetLabel.Text = $"Next Server Reset: {TDFConnections.nextServerReset}";
                DailyResetLabel.Text = $"Next Daily Reset: {TDFConnections.nextDailyReset}";

                Thread.Sleep(200);

                TDFConnections.ServerReset(true);
                InitializeDow30Data();
                timer1.Enabled = true;
                resetting = false;

            }

            if (DateTime.Now > TDFConnections.nextDailyReset && !resetting)
            {
                resetting = true;
                WatchdogTimer.Enabled = false;
                
                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(TDFGlobals.ServerID);
                TDFConnections.nextDailyReset = TDFConnections.GetNextDailyResetTime(sr);
                DailyResetLabel.Text = $"Next Daily Reset: {TDFConnections.nextDailyReset}";

                Thread.Sleep(200);
                
                TDFConnections.ServerReset(false);
                timer1.Enabled = false;
                InitializeDow30Data();
                timer1.Enabled = true;
                resetting = false;
                //TDFConnections.nextDailyReset = DateTime.Now.AddMinutes(5);

            }

            if (zipperFlag == true && DateTime.Now > zipperEmailSent.AddDays(1))
                zipperFlag = false;

        }

        
        public void UnsubscribeAll()
        {
            foreach (symbolData sd in TDFGlobals.symbols)
            {
                string sym = sd.symbolFull;
                string queryStr = $"DELETE FROM SUBSCRIPTION_TABLE WHERE channelName = DYNAMIC_QUOTES AND usrSymbol = {quot}{sym}{quot}";
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.DATA_REQUEST, 1);
                TDFConnections.TRSendCommand(outputbuf);
                //TRSendCommand(outputbuf);
                listBox1.Items.Add(queryStr);
                Thread.Sleep(50);
                
            }
        }
        
    

        private void gbTime_Enter(object sender, EventArgs e)
        {

        }

        
        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            if (timer1.Enabled)
                button6.Text = "Pause";
            else
                button6.Text = "GO";
        }
        public void UpdateZipperDataFile1()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(zipperFilePath + "ZipperDataFile.xml");
            XmlNodeList symbolNodes = xmlDoc.SelectNodes("//SYMBOLS/SYMBOL");

            int i = 0;

            foreach (XmlNode sym in symbolNodes)
            {
                
                sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();

                if (TDFGlobals.symbols[i].netChg > 0)
                {
                    sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();
                    sym.Attributes["change"].Value = TDFGlobals.symbols[i].netChg.ToString();
                    sym.Attributes["arrow"].Value = "up.jpg";
                }
                else if (TDFGlobals.symbols[i].netChg < 0)
                {
                    sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();
                    float absChange = Math.Abs(TDFGlobals.symbols[i].netChg);
                    sym.Attributes["change"].Value = absChange.ToString();
                    sym.Attributes["arrow"].Value = "down.jpg";
                }
                else if (TDFGlobals.symbols[i].netChg == 0)
                {
                    sym.Attributes["value"].Value = TDFGlobals.symbols[i].trdPrc.ToString();
                    sym.Attributes["change"].Value = "UNCH";
                }
                i++;
                
            }
            xmlDoc.Save(zipperFilePath + "ZipperDataFile.xml");
            
        }


        public void UpdateZipperDataFile()
        {
            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(zipperFilePath + "ZipperDataFile.xml");
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("SYMBOLS");
                double value;
                string str;

                for (int i = 0; i < 3; i++)
                {

                    xmlWriter.WriteStartElement("SYMBOL");
                    value = TDFGlobals.symbols[i].trdPrc;
                    str = TDFGlobals.symbols[i].trdPrc.ToString("#####.##");
                    //xmlWriter.WriteAttributeString("name", TDFGlobals.symbols[i].name.ToString());
                    //xmlWriter.WriteAttributeString("value", TDFGlobals.symbols[i].trdPrc.ToString("#####.##"));

                    xmlWriter.WriteAttributeString("name", TDFGlobals.symbols[i].company_Name.ToString());
                    xmlWriter.WriteAttributeString("value", str);
                    if (TDFGlobals.symbols[i].netChg > 0)
                    {
                        value = TDFGlobals.symbols[i].netChg;
                        str = TDFGlobals.symbols[i].netChg.ToString("#####.##");
                        //xmlWriter.WriteAttributeString("change", TDFGlobals.symbols[i].netChg.ToString("#####.##"));

                        xmlWriter.WriteAttributeString("change", str);
                        xmlWriter.WriteAttributeString("arrow", "up.jpg");
                    }
                    else if (TDFGlobals.symbols[i].netChg < 0)
                    {
                        float absChange = Math.Abs(TDFGlobals.symbols[i].netChg);
                        str = absChange.ToString("#####.##");
                        //xmlWriter.WriteAttributeString("change", absChange.ToString("#####.##"));
                        xmlWriter.WriteAttributeString("change", str);
                        xmlWriter.WriteAttributeString("arrow", "down.jpg");
                    }
                    else if (TDFGlobals.symbols[i].netChg == 0)
                    {
                        xmlWriter.WriteAttributeString("change", "UNCH");
                    }

                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
            catch
            {
                if (zipperFlag == false)
                {
                    zipperFlag = true;
                    string msg = "[" + DateTime.Now + "] TDFDow30 write error. Error writing to Zipper Data File.";
                    SendEmail(msg);
                    zipperEmailSent = DateTime.Now;
                    log.Debug("TDFDow30 write error. Error writing to Zipper Data File.");
                }
            }
        }

        
        private void WatchdogTimer_Tick(object sender, EventArgs e)
        {
            if (timerFlag == false)
            {
                //timer1.Enabled = false;
                timerFlag = true;
                string msg = "[" + DateTime.Now + "] TDFDow30 response error. Data requested with no response.";
                //SendEmail(msg);
                timerEmailSent = DateTime.Now;
                TDFConnections.DisconnectFromTDF();
                ResetTimer.Enabled = true;
            }
            log.Debug("TDFDow30 response error. Data requested with no response.");

        }

        private void ResetTimer_Tick(object sender, EventArgs e)
        {
            ResetTimer.Enabled = false;
            log.Debug("Reset Timer fired.");
            TDFConnections.ServerReset(false);
            timer1.Enabled = true;
        }

        public bool MarketOpenStatus()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            DateTime timeNow = DateTime.Now;
            bool marketOpenStat;

            bool weekday;
            if (timeNow.DayOfWeek != DayOfWeek.Saturday && timeNow.DayOfWeek != DayOfWeek.Sunday)
                weekday = true;
            else
                weekday = false;

            bool todayIsHoliday = false;

            for (int i = 0; i < marketHolidays.Count; i++)
            {
                if (marketHolidays[i].holiDate == DateTime.Today)
                    todayIsHoliday = true;
            }

            if (currentTime > marketOpen && currentTime < marketClose && weekday == true && todayIsHoliday == false)
            {


                marketOpenStat = true;
                
            }
            else
                marketOpenStat = false;

            return marketOpenStat;

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void ServerTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }

}



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

        public string logResp = "";
        public ushort session_ID = 0xffff;


        //public byte[] msgBuf = new byte[5000];
        //public Int32 msgBufLen = 0;
        public Int32[,] CatalogData = new int[150, 60];
        //public Int16 numCat = 0;
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
        public string spName = "";



        public class XMLUpdateEventArgs : EventArgs
        {
            public string XML { get; set; }
        }

        //ChartForm cf = new ChartForm();
        //private readonly WindowsFormsSynchronizationContext syncContext;
        //private readonly SynchronizationContext syncContext;


        #endregion

        #region Collection, bindilist & variable definitions

        /// <summary>
        /// Define classes for collections and logic
        /// </summary>

        // Declare TCP client sockets for Thomson Reuters communications
        public AsyncClientSocket.ClientSocket TRClientSocket;
        bool TRConnected = false;


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
        }

        public event EventHandler<SymbolUpdateEventArgs> SymbolDataUpdated;
        //public event EventHandler<ChartLiveUpdateEventArgs> ChartDataUpdated;
        //public event EventHandler<XMLUpdateEventArgs> XMLDataUpdated;
        //public event EventHandler<ChartClosedEventArgs> ChartClosed;

        protected virtual void OnSymbolDataUpdated(SymbolUpdateEventArgs e)
        {

            EventHandler<SymbolUpdateEventArgs> evntH = SymbolDataUpdated;
            if (evntH != null)
                evntH(this, e);


            //SymbolDataUpdated?.Invoke(this, e);
        }
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
                spName = Properties.Settings.Default.spUpdate;
                dynamic = Properties.Settings.Default.Dynamic;
                zipperFilePath = Properties.Settings.Default.ZipperFilePath;
                debugMode = Properties.Settings.Default.DebugMode;


                disconnectTime = DateTime.Today + Properties.Settings.Default.Reset_Connection;
                if (DateTime.Now > disconnectTime)
                    disconnectTime = disconnectTime.AddDays(1);
                refTime = DateTime.Today.AddHours(1);
                
                
                IPTextBox.Text = IPAddress;
                PortTextBox.Text = Port;
                UserTextBox.Text = UserName;
                PWTextBox.Text = PW;

                TDFGlobals.showAllFields = false;

                for (int i = 0; i < 150; i++)
                {
                    for (int j = 0; j < 60; j++)
                    {
                        CatalogData[i, j] = 0;
                    }
                }

                // fields requested in hotboards
                TDFGlobals.starredFields.Add("trdPrc"); // 0
                TDFGlobals.starredFields.Add("netChg"); // 1
                TDFGlobals.starredFields.Add("ycls"); //2
                TDFGlobals.starredFields.Add("pcntChg"); //3
                /*
                TDFGlobals.starredFields.Add("hi"); // 4
                TDFGlobals.starredFields.Add("lo"); // 5
                TDFGlobals.starredFields.Add("annHi"); // 6
                TDFGlobals.starredFields.Add("annLo");// 7
                TDFGlobals.starredFields.Add("cumVol"); // 8
                TDFGlobals.starredFields.Add("peRatio"); // 9
                TDFGlobals.starredFields.Add("eps"); // 10
                TDFGlobals.starredFields.Add("ask"); // 11
                TDFGlobals.starredFields.Add("bid"); // 12
                TDFGlobals.starredFields.Add("lastActivity"); // 13
                TDFGlobals.starredFields.Add("lastActivityNetChg"); // 14
                TDFGlobals.starredFields.Add("lastActivityPcntChg"); // 15
                TDFGlobals.starredFields.Add("divAnn"); // 16
                TDFGlobals.starredFields.Add("intRate"); // 17
                TDFGlobals.starredFields.Add("bidYld"); // 18
                TDFGlobals.starredFields.Add("bidNetChg"); // 19
                TDFGlobals.starredFields.Add("askYld"); // 20
                TDFGlobals.starredFields.Add("bidYldNetChg"); // 21
                TDFGlobals.starredFields.Add("yrClsPrc"); // 22
                TDFGlobals.starredFields.Add("monthClsPrc"); //23
                TDFGlobals.starredFields.Add("mktCap"); //24
                TDFGlobals.starredFields.Add("opn"); // 25
                TDFGlobals.starredFields.Add("yld"); // 26
                TDFGlobals.starredFields.Add("prcFmtCode"); // 27
                TDFGlobals.starredFields.Add("companyShrsOutstanding"); // 28
                TDFGlobals.starredFields.Add("sectyType"); // 29
                TDFGlobals.starredFields.Add("symbol"); // 30
                */

                // Log application start
                log.Debug("\r\n\r\n*********** Starting TDFDow30 **********\r\n");



                //XMLDataUpdated += new EventHandler<XMLUpdateEventArgs>(DisplayXMLData);
                //SymbolDataUpdated += new EventHandler<SymbolUpdateEventArgs>(SymbolDataUpdated);
                //ChartDataUpdated += new EventHandler<ChartLiveUpdateEventArgs>(ChartDataUpdated);
                //ChartClosed += new EventHandler<ChartClosedEventArgs>(ChartClosed);

                TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();
                TDFproc.sendBuf += new SendBuf(TRSendCommand);
                TODTimer.Enabled = true;

                
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("frmMain Exception occurred during main form load: " + ex.Message);
                //log.Debug("frmMain Exception occurred during main form load", ex);
            }
            TODTimer.Enabled = true;
            ConnectToTDF();
        }



        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectToTDF();
        }

        public void ConnectToTDF()
        {
            // Build Logon Message
            string queryStr = "LOGON USERNAME" + "=\"" + UserTextBox.Text + "\" PASSWORD=\"" +
                PWTextBox.Text + "\"";


            

            // Instantiate and setup the client sockets
            // Establish the remote endpoints for the sockets
            System.Net.IPAddress TRIpAddress = System.Net.IPAddress.Parse(IPAddress);
            TRClientSocket = new ClientSocket(TRIpAddress, Convert.ToInt32(Port));

            // Initialize event handlers for the sockets
            TRClientSocket.DataReceived += TRDataReceived;
            TRClientSocket.ConnectionStatusChanged += TRConnectionStatusChanged;

            // Connect to the TRClientSocket; call-backs for connection status will indicate status of client sockets
            TRClientSocket.AutoReconnect = true;
            TRClientSocket.Connect();

            System.Threading.Thread.Sleep(1000);
            if (TRConnected == true)
            {
                pictureBox2.Visible = true;
            }

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.LOGON_REQUEST, 0);
            TRSendCommand(outputbuf);

            int n = 0;
            while (loggedIn == false && n < 50)
            {
                n++;
                System.Threading.Thread.Sleep(100);

            }
            TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();

            TDFproc.sendBuf += new SendBuf(TRSendCommand);
            lblLogResp.Text = logResp;
            TDFproc.GetCataloger();
            //label7.Text = "Num Catalogs: " + numCat.ToString();

            TDFproc.GetFieldInfoTable();
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
            dataGridView1.DataSource = Dow30Data;

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
                sd1.name = sd.DisplayName.ToUpper();
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
    
        public void IssueDynamicSubscriptionQuery(string symbolStr, uint seq)
        {
            string fieldList = "trdPrc, netChg, pcntChg"; 
            //string query = "SELECT " + fieldList + " FROM DYNAMIC_QUOTES WHERE usrSymbol= \"" + symbolStr + "\"";
            string query = $"SELECT {fieldList} FROM DYNAMIC_QUOTES WHERE usrSymbol= {quot}{symbolStr}{quot}";
            if (TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, seq);

                TRSendCommand(outputbuf);
                listBox1.Items.Add(query);

            }
        }
        
        #region Socket Handlers
        // Handler for data received back from TRclientsocket
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
            TDFproc.sendBuf += new SendBuf(TRSendCommand);

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

                        TDFproc.ProcessKeepAliveRequest(TRmessage);
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
                            TDFproc.ProcessKeepAliveRequest(TRmessage);
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
                                TDFproc.ProcessKeepAliveRequest(TRmessage);
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




        // Handler for source & destination MSE connection status change
        public void TRConnectionStatusChanged(ClientSocket sender, ClientSocket.ConnectionStatus status)
        {
            // Set status
            if (status == ClientSocket.ConnectionStatus.Connected)
            {
                TRConnected = true;
            }
            else
            {
                TRConnected = false;
            }
            if (debugMode)
                messages.Add("status: " + status.ToString());

            // Send to log - DEBUG ONLY
            log.Debug("TR Connection Status: " + status.ToString());
        }

        // Send a command to TDF
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
                log.Error("Error occurred while trying to send data to TR client port: " + ex.Message);
            }
        }
        #endregion;
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connection = $"SELECT * FROM {dbTableName}";
            Dow30Data = Dow30Database.Dow30DB.GetSymbolDataCollection(connection, dbConn);
            dataGridView1.DataSource = Dow30Data;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool first = true;
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
                sd1.queryType = (int)QueryTypes.Portfolio_Mgr;
                sd1.queryStr = "";
                sd1.symbol = sd.SubscribeSymbol;
                sd1.seqId = 5;
                sd1.updated = DateTime.Now;
                TDFGlobals.symbols.Add(sd1);
            }
            label1.Text = symbolListStr;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            stopWatch.Start();
            string fieldList = "trdPrc, netChg, pcntChg";
            string query = $"SELECT {fieldList} FROM PORTFOLIO_MGR WHERE usrSymbol IN ({symbolListStr})";
            //if (TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                TRSendCommand(outputbuf);
            }
            timer1.Enabled = true;
        }

        public void GetDow30Data()
        {
            stopWatch.Start();
            string fieldList = "trdPrc, netChg, pcntChg";
            string query = $"SELECT {fieldList} FROM PORTFOLIO_MGR WHERE usrSymbol IN ({symbolListStr})";
            if (TRConnected)
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, query, TDFconstants.DATA_REQUEST, 5);

                TRSendCommand(outputbuf);
                WatchdogTimer.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string s = "";
            string sym = "";
            string oldSym = "";

            if (dynamic == false)
            {
                GetDow30Data();
                Thread.Sleep(50);
            }

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

                    if (dynamic == false && debugMode == true)
                        DisplayResults(TDFGlobals.financialResults, i);

                    if (TDFGlobals.financialResults.Count > 0)
                        TDFProcessingFunctions.SetSymbolData(TDFGlobals.financialResults, i, symbolIndex);

                }
                TDFGlobals.financialResults.Clear();

                UpdateAllSymbols();

                string connection = $"SELECT * FROM {dbTableName}";
                Dow30Data = Dow30Database.Dow30DB.GetSymbolDataCollection(connection, dbConn);
                dataGridView1.DataSource = Dow30Data;
                dataGridView1.ClearSelection();

                foreach (DataGridViewRow row in dataGridView1.Rows)
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
                UpdateZipperDataFile();
                //label1.Text = $"Elapsed: {ts.TotalMilliseconds.ToString()} msec";
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

        public void UpdateAllSymbols()
        {
            for(int i = 0; i < TDFGlobals.symbols.Count; i++)
            {
                symbolData sd = new symbolData();
                sd = TDFGlobals.symbols[i];
                if (Dow30Data[i].Last != sd.netChg)
                    UpdateDB(sd);
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
                                    log.Error("UpdateData- SQL Command Exception occurred: " + ex.Message);
                                    log.Debug("UpdateData- SQL Command Exception occurred", ex);
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

        private void button4_Click(object sender, EventArgs e)
        {
            SendEmail("[" + DateTime.Now + "] " + "TDFDow30 test message. Pleae ignore.");
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dynamic)
                UnsubscribeAll();
            Logoff();
        }
        private void Logoff()
        {
            // Build Logon Message
            string queryStr = "LOGOFF";

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.LOGOFF_REQUEST, 0);
            TRSendCommand(outputbuf);
        }
        public void DisconnectFromTDF()
        {
            Logoff();
            Thread.Sleep(200);
            TRClientSocket.Disconnect();
            Thread.Sleep(200);
            
        }

        private void TODTimer_Tick(object sender, EventArgs e)
        {
            timeOfDayLabel.Text = DateTime.Now.ToString("MMM d, yyyy -- h:mm:ss tt");

            if (TRConnected)
                pictureBox2.Visible = true;
            else
                pictureBox2.Visible = false;

            lblLogResp.Text = logResp;

            for (int i = 0; i< messages.Count; i++)
            {
                if (debugMode)
                    listBox1.Items.Add(messages[i]);

            }
            messages.Clear();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;


            if (timerFlag == true && DateTime.Now > refTime)
                timerFlag = false;

            if (resetFlag == true && DateTime.Now > refTime)
                resetFlag = false;


            if (resetComplete == true && DateTime.Now > refTime)
                resetComplete = false;


            if (resetComplete == false && resetting == false && DateTime.Now > disconnectTime)
                ResetTDFConnection();
            


        }

        public void ResetTDFConnection()
        {
            log.Debug("Resetting TDF Connection");
            resetting = true;
            timer1.Enabled = false;
            if (dynamic)
                UnsubscribeAll();

            DisconnectFromTDF();
            Thread.Sleep(1000);
            TDFGlobals.symbols.Clear();
            TDFGlobals.financialResults.Clear();
            ConnectToTDF();
            resetting = false;
            if (TRConnected == true)
            {
                resetComplete = true;
                disconnectTime = disconnectTime.AddDays(1);
                refTime = refTime.AddDays(1);
                timer1.Enabled = true;
                log.Debug("Reset complete");
            }
            else
            {
                if (resetFlag == false)
                {
                    resetFlag = true;
                    string msg = "[" + DateTime.Now + "] TDFDow30 reset error. Failed to reconnect after timed disconnect.";
                    SendEmail(msg);
                }
            }
        }

        public void UnsubscribeAll()
        {
            foreach (symbolData sd in TDFGlobals.symbols)
            {
                string sym = sd.symbolFull;
                string queryStr = $"DELETE FROM SUBSCRIPTION_TABLE WHERE channelName = DYNAMIC_QUOTES AND usrSymbol = {quot}{sym}{quot}";
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                byte[] outputbuf = itfHeaderAccess.Build_Outbuf(stdHeadr, queryStr, TDFconstants.DATA_REQUEST, 1);
                TRSendCommand(outputbuf);
                listBox1.Items.Add(queryStr);
                Thread.Sleep(50);
                
            }
        }
        
    

        private void gbTime_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ResetTDFConnection();
            
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

            bool unchFlag = false;
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
                    unchFlag = true;
                }
                i++;
                
            }
            xmlDoc.Save(zipperFilePath + "ZipperDataFile.xml");
            
        }


        public void UpdateZipperDataFile()
        {
            
            bool unchFlag = false;
            
            XmlWriter xmlWriter = XmlWriter.Create(zipperFilePath + "ZipperDataFile.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("SYMBOLS");
            
            for (int i = 0; i < 3; i++)
            {
                xmlWriter.WriteStartElement("SYMBOL");

                xmlWriter.WriteAttributeString("name", TDFGlobals.symbols[i].name.ToString());
                xmlWriter.WriteAttributeString("value", TDFGlobals.symbols[i].trdPrc.ToString());
                if (TDFGlobals.symbols[i].netChg > 0)
                {
                    xmlWriter.WriteAttributeString("change", TDFGlobals.symbols[i].netChg.ToString());
                    xmlWriter.WriteAttributeString("arrow", "up.jpg");
                }
                else if (TDFGlobals.symbols[i].netChg < 0)
                {
                    float absChange = Math.Abs(TDFGlobals.symbols[i].netChg);
                    xmlWriter.WriteAttributeString("change", absChange.ToString());
                    xmlWriter.WriteAttributeString("arrow", "down.jpg");
                }
                else if (TDFGlobals.symbols[i].netChg == 0)
                {
                    xmlWriter.WriteAttributeString("change", "UNCH");
                    unchFlag = true;
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            
        }



        private void button7_Click(object sender, EventArgs e)
        {
            UpdateZipperDataFile();
        }

        private void WatchdogTimer_Tick(object sender, EventArgs e)
        {
            if (timerFlag == false)
            {
                timerFlag = true;
                string msg = "[" + DateTime.Now + "] TDFDow30 response error. Data requested with no response.";
                SendEmail(msg);
            }
        }
    }

}



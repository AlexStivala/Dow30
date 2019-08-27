using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace TDFInterface
{
    public class TDFConnections
    {

        public const string quot = "\"";
        public static System.Timers.Timer ReconnectTimer = new System.Timers.Timer();
        public static  DateTime nextServerReset;
        public static DateTime nextDailyReset;



        #region Logger instantiation - uses reflection to get module name
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Logging & status setup
        // This method used to implement IAppender interface from log4net; to support custom appends to status strip
        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            // Set text on status bar only if logging level is DEBUG or ERROR
            if ((loggingEvent.Level.Name == "ERROR") | (loggingEvent.Level.Name == "DEBUG"))
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
        #endregion


        public TDFConnections()
        {
            ReconnectTimer.Elapsed += new ElapsedEventHandler(ReconnectTimer_Tick);
        }


        public static void ConnectToTDF(int serverId)
        {
            // Build Logon Message
            //string queryStr = "LOGON USERNAME" + "=\"" + UserTextBox.Text + "\" PASSWORD=\"" +
            //PWTextBox.Text + "\"";

            try
            {
                ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
                TDFProcessingFunctions.sendBuf += new SendBuf(TRSendCommand);

                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(serverId);

                // Instantiate and setup the client sockets
                // Establish the remote endpoints for the sockets
                System.Net.IPAddress TRIpAddress = System.Net.IPAddress.Parse(sr.IPAddress);
                TDFGlobals.TRClientSocket = new AsyncClientSocket.ClientSocket(TRIpAddress, Convert.ToInt32(sr.Port));

                // Initialize event handlers for the sockets
                TDFGlobals.TRClientSocket.DataReceived += TRDataReceived;
                TDFGlobals.TRClientSocket.ConnectionStatusChanged += TRConnectionStatusChanged;

                // Connect to the TRClientSocket; call-backs for connection status will indicate status of client sockets
                TDFGlobals.TRClientSocket.AutoReconnect = false;
                TDFGlobals.TRClientSocket.Connect();

                int n = 0;
                bool done = false;

                while (!TDFGlobals.TRConnected)
                {
                    while (n < 2000 && !done)
                    {
                        System.Threading.Thread.Sleep(10);
                        if (TDFGlobals.TRConnected == true)
                        {
                            // Connected to TDF
                            log.Info("Connected to TDF...     Logging On....");
                            done = true;
                            string queryStr = $"LOGON USERNAME={quot}{sr.UserId}{quot} PASSWORD={quot}{sr.PW}{quot}";
                            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(TDFProcessingFunctions.stdHeadr, queryStr, TDFconstants.LOGON_REQUEST, 0);
                            TRSendCommand(outputbuf);
                        }
                        n++;
                    }

                    if (!done)
                    {
                        // Not Connected to TDF
                        n = 0;
                        log.Error("Error: Did not connect.");
                    }
                }

                n = 0;
                done = false;
                while (TDFGlobals.logResp.Length == 0)
                {
                    while (n < 2000 && !TDFGlobals.loggedIn)
                    {
                        // Logged on to TDF
                        System.Threading.Thread.Sleep(10);
                        if (TDFGlobals.logResp.Length > 5)
                        {
                            //log.Info(logResp);
                            System.Threading.Thread.Sleep(100);
                            //done = true;
                        }
                        n++;
                    }

                    if (!TDFGlobals.loggedIn)
                    {
                        // Not Logged on to TDF
                        n = 0;
                        log.Error("Error: Not logged on.");

                    }
                }

                //lblLogResp.Text = logResp;
                
                TDFProcessingFunctions.GetCataloger();
                System.Threading.Thread.Sleep(100);
                //label7.Text = "Num Catalogs: " + numCat.ToString();
                log.Info($"Num Catalogs: {TDFGlobals.numCat}");

                TDFProcessingFunctions.GetFieldInfoTable();
                System.Threading.Thread.Sleep(1000);
                Int32 cnt = 0;

                for (int i = 0; i < TDFGlobals.field_Info_Table.Length; i++)
                {
                    if (TDFGlobals.field_Info_Table[i].fieldId > 0)
                        cnt++;
                }
                log.Info($"Num Fields: {cnt}");

                ReconnectTimer.Interval = 120000;
                ReconnectTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                // Log the error 
                log.Error($"Error connecting to Thomson: {ex}");
            }

        }

        // Handler for source & destination MSE connection status change
        public static void TRConnectionStatusChanged(AsyncClientSocket.ClientSocket sender, AsyncClientSocket.ClientSocket.ConnectionStatus status)
        {
            // Set status
            if (status == AsyncClientSocket.ClientSocket.ConnectionStatus.Connected)
            {
                TDFGlobals.TRConnected = true;
            }
            else
            {
                TDFGlobals.TRConnected = false;
            }
            
            log.Info("TR Connection Status: " + status.ToString());
        }

        public static void TRDataReceived(AsyncClientSocket.ClientSocket sender, byte[] data)
        {
            //TDFProcessingFunctions TDFproc = new TDFProcessingFunctions();
            //TDFproc.TDFDataReceived(sender, data);
            TDFProcessingFunctions.TDFDataReceived(sender, data);
        }

        // Send a command to TDF
        public static void TRSendCommand(byte[] outbuf)
        {
            try
            {
                // Send the data; terminiate with CRLF
                TDFGlobals.TRClientSocket.Send(outbuf);
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("Error occurred while trying to send data to TR client port: " + ex.Message);
            }
        }

        public static void Logoff()
        {
            // Build Logon Message
            string queryStr = "LOGOFF";
            log.Info("Logging off TDF...");

            ItfHeaderAccess itfHeaderAccess = new ItfHeaderAccess();
            byte[] outputbuf = itfHeaderAccess.Build_Outbuf(TDFProcessingFunctions.stdHeadr, queryStr, TDFconstants.LOGOFF_REQUEST, 0);
            TRSendCommand(outputbuf);
        }
        
        public static void DisconnectFromTDF()
        {
            Logoff();
            Thread.Sleep(200);
            TDFGlobals.TRClientSocket.Disconnect();
            TDFGlobals.TRConnected = false;
            //pictureBox2.Visible = false;
            Thread.Sleep(1000);
        }

        public static void ServerReset(bool isServerReset)
        {

            // Daily reset Values
            int resetMinutes = 2;
            string resetType = "Daily";

            if (isServerReset)
            {
                //Server reset values
                resetMinutes = 60;
                resetType = "Server";
            }

            log.Debug($"Starting {resetType} Reset procedure.....");
            //resetting = true;
            //timer1.Enabled = false;

            TDFProcessingFunctions TDFProc = new TDFProcessingFunctions();
            if (TDFGlobals.dynamic)
            {
                TDFProc.UnsubscribeAll();
                log.Debug("Unsubscribe complete.");
            }

            DisconnectFromTDF();

            TDFGlobals.symbols.Clear();
            TDFGlobals.financialResults.Clear();
            TDFGlobals.TRdata.Clear();
            TDFGlobals.dataLeft = 0;

            ReconnectTimer.Interval = 60000 * resetMinutes;
            ReconnectTimer.Enabled = true;
            log.Debug($"Reconnecting in {resetMinutes} minutes.....");

        }

        private static void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            ReconnectTimer.Enabled = false;
            log.Debug("ReconnectTimer fired. ");
            if (!TDFGlobals.TRConnected)
            {
                ConnectToTDF(TDFGlobals.ServerID);
                Thread.Sleep(2000);
                if (TDFGlobals.TRConnected == true)
                {
                    log.Debug("Reset complete");
                }
                else
                {
                    string msg = $"[{DateTime.Now}] {System.Reflection.Assembly.GetEntryAssembly().GetName()} reset error. Failed to reconnect after timed disconnect.";
                    //SendEmail(msg);
                    log.Debug($"{System.Reflection.Assembly.GetEntryAssembly().GetName()} reset error. Failed to reconnect after timed disconnect.");
                }
            }
            else
                log.Debug(">>>>>>>>>>>>>>>>>>>>>> ReconnectTimer error: Already connected. ");
        }

        public static DateTime GetNextServerResetTime(MarketModel.ServerReset sr)
        {
            try
            {
                int weekNo = sr.weekNo;
                DateTime now = DateTime.Now;
                DayOfWeek resetDay = (DayOfWeek)sr.resetDay;
                DateTime resetTime = sr.resetTime;

                int srDate = FindDay(now.Year, now.Month, resetDay, weekNo);
                nextServerReset = new DateTime(now.Year, now.Month, srDate, resetTime.Hour, resetTime.Minute, resetTime.Second);

                if (now.AddMinutes(3) > nextServerReset)
                    nextServerReset = nextServerReset.AddDays(28);

                nextServerReset = nextServerReset.AddMinutes(-2);
                //ServerResetLabel.Text = $"Next Server Reset: {nextServerReset}";

            }
            catch (Exception ex)
            {
                log.Error($"GetNextServerResetTime Error: {ex}");
            }
            return nextServerReset;

        }

        public static DateTime GetNextDailyResetTime(MarketModel.ServerReset sr)
        {
            DateTime resetTime = sr.resetTime;
            DateTime disconnectTime = sr.resetTime;

            try
            {
                
                disconnectTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, resetTime.Hour, resetTime.Minute, resetTime.Second);
                if (DateTime.Now.AddMinutes(3) >= disconnectTime)
                    disconnectTime = disconnectTime.AddDays(1);

                //DailyResetLabel.Text = $"Next Daily Reset: {disconnectTime}";
            }
            catch (Exception ex)
            {
                log.Error($"GetNextDailyResetTime Error: {ex}");
            }
            return disconnectTime;

        }

        //For example to find the day for 2nd Friday, February, 2016
        //=>call FindDay(2016, 2, DayOfWeek.Friday, 2)
        public static int FindDay(int year, int month, DayOfWeek Day, int occurance)
        {

            if (occurance <= 0 || occurance > 5)
                throw new Exception("Occurance is invalid");

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            //Substract first day of the month with the required day of the week 
            var daysneeded = (int)Day - (int)firstDayOfMonth.DayOfWeek;
            //if it is less than zero we need to get the next week day (add 7 days)
            if (daysneeded < 0) daysneeded = daysneeded + 7;
            //DayOfWeek is zero index based; multiply by the Occurance to get the day
            var resultedDay = (daysneeded + 1) + (7 * (occurance - 1));

            if (resultedDay > (firstDayOfMonth.AddMonths(1) - firstDayOfMonth).Days)
                throw new Exception(String.Format("No {0} occurance(s) of {1} in the required month", occurance, Day.ToString()));

            return resultedDay;
        }

        public static void TDFSetup()
        {
            TDFProcessingFunctions.InitializeSymbolFields();
            ConnectToTDF(TDFGlobals.ServerID);
            Thread.Sleep(2000);
            log.Info("Setup complete");

        }

        public static void TDFResetCheck()
        {
            // if server is scheduled for reset - Get next time for both resets
            if (DateTime.Now > nextServerReset)
            {
                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(TDFGlobals.ServerID);
                nextServerReset = GetNextServerResetTime(sr);
                nextDailyReset = GetNextDailyResetTime(sr);
                
                ServerReset(true);
            }

            if (DateTime.Now > nextDailyReset)
            {
                MarketModel.ServerReset sr = MarketFunctions.GetServerResetSched(TDFGlobals.ServerID);
                nextDailyReset = GetNextDailyResetTime(sr);
                ServerReset(false);
            }

        }

    }


}

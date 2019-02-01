using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using TDFInterface;
using log4net;


namespace Dow30Database
{
    public class Dow30DB
    {

        public class Dow30symbolData
        {
            public int SymbolType { get; set; }
            public string SubscribeSymbol { get; set; }
            public string DisplaySymbol { get; set; }
            public string DisplayName { get; set; }
            public float Last { get; set; }
            public float Change { get; set; }
            public float PercentChange { get; set; }
            public DateTime Updated { get; set; }
            
        }
        public class MarketHolidays
        {
            public string holiday { get; set; }
            public DateTime holiDate { get; set; }
        }



        public static DataTable GetDBData(string cmdStr, string dbConnection)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    // Create the command and set its properties
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandText = cmdStr;
                            //cmd.Parameters.Add("@StackID", SqlDbType.Float).Value = stackID;
                            sqlDataAdapter.SelectCommand = cmd;
                            sqlDataAdapter.SelectCommand.Connection = connection;
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                            // Fill the datatable from adapter
                            sqlDataAdapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                //log.Error("GetDBData Exception occurred: " + ex.Message);
                //log.Debug("GetDBData Exception occurred", ex);
            }

            return dataTable;
        }

        public static int SQLExec(string cmdStr, string dbConnection)
        {
            int numRowsAffected = -2;
            try
            {
                // Instantiate the connection
                using (SqlConnection connection = new SqlConnection(dbConnection))
                {
                    // Create the command and set its properties
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            cmd.CommandText = cmdStr;
                            sqlDataAdapter.SelectCommand = cmd;
                            sqlDataAdapter.SelectCommand.Connection = connection;
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
                            numRowsAffected = sqlDataAdapter.SelectCommand.ExecuteNonQuery();
                        }
                        connection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                //log.Error("GetDBData Exception occurred: " + ex.Message);
                //log.Debug("GetDBData Exception occurred", ex);
                numRowsAffected = -1;

            }
            return numRowsAffected;
        }


        public static BindingList<Dow30symbolData> GetSymbolDataCollection(string cmdStr, string dbConnection)
        {
            DataTable dataTable;

            // Clear out the current collection
            //candidateData.Clear();
            BindingList<Dow30symbolData> Dow30 = new BindingList<Dow30symbolData>();


            try
            {
                dataTable = GetDBData(cmdStr, dbConnection);

                foreach (DataRow row in dataTable.Rows)
                {
                    var sd = new Dow30symbolData()
                    {
                        SymbolType = Convert.ToInt32(row["SymbolType"] ?? ""),
                        SubscribeSymbol = row["SubscribeSymbol"].ToString() ?? "",
                        DisplaySymbol = row["DisplaySymbol"].ToString() ?? "",
                        DisplayName = row["DisplayName"].ToString() ?? "",
                        Last = Convert.ToSingle(row["Last"] ?? ""),
                        Change = Convert.ToSingle(row["Change"] ?? ""),
                        PercentChange = Convert.ToSingle(row["PercentChange"] ?? ""),
                        Updated = Convert.ToDateTime(row["Updated"] ?? ""),                       
                    };
                    Dow30.Add(sd);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
                // Log error
                //log.Error("GetCandidateDataCollection Exception occurred: " + ex.Message);
                //log.Debug("GetCandidateDataCollection Exception occurred", ex);
            }
            // Return 
            return Dow30;
        }
        public static List<MarketHolidays> GetHolidays(string cmdStr, string dbConnection)
        {
            DataTable dataTable;

            // Clear out the current collection
            //candidateData.Clear();
            List<MarketHolidays> holidays = new List<MarketHolidays>();


            try
            {
                dataTable = GetDBData(cmdStr, dbConnection);

                foreach (DataRow row in dataTable.Rows)
                {
                    var hol = new MarketHolidays()
                    {
                        holiday = row["Holiday"].ToString() ?? "",
                        holiDate = Convert.ToDateTime(row["holiDate"] ?? ""),

                    };
                    holidays.Add(hol);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
                // Log error
                //log.Error("GetCandidateDataCollection Exception occurred: " + ex.Message);
                //log.Debug("GetCandidateDataCollection Exception occurred", ex);
            }
            // Return 
            return holidays;
        }

    }
}

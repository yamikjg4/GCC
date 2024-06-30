using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GlobalCalendar.MVC.DAL
{
    public class SQLSERVER_HELPER
    {
        private static readonly string connString;
        private static string stagingConnString;
        static SQLSERVER_HELPER()
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            connString = configuration.GetConnectionString("TBSConnStr");
            stagingConnString = configuration.GetConnectionString("TBSSAPConn");
        }
        /* private static string stagingConnString = ConfigurationManager.ConnectionStrings["TBSSAPConn"].ConnectionString;*/

        #region Call Stored Procedure
        public static int CallStoredProcedure(string SPName, Dictionary<string, object> parameters)
        {
            var rowsAffected = 0;
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                using (var sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    using (var sqlCmd = new SqlCommand(SPName, sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (var item in parameters)
                            {
                                sqlCmd.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }

                        rowsAffected = sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
#pragma warning restore CS0168 // Variable is declared but never used
            return rowsAffected;
        }
        #endregion

        #region GetData from StoredProcedure with Parameters
        public static DataSet GetData(string SPName, Dictionary<string, object> parameters)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            DataSet dsData = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            try
            {
                using (var sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    using (var sqlCmd = new SqlCommand(SPName, sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
                                sqlCmd.Parameters.AddWithValue(p.Key, p.Value);
                            }
                        }

                        using (var sqlDa = new SqlDataAdapter(sqlCmd))
                        {
                            dsData = new DataSet();
                            sqlDa.Fill(dsData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dsData;
        }

        public static DataSet GetSAPDBData(string SPName, Dictionary<string, object> parameters)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            DataSet dsData = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            try
            {
                using (var sqlConn = new SqlConnection(stagingConnString))
                {
                    sqlConn.Open();
                    using (var sqlCmd = new SqlCommand(SPName, sqlConn))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
                                sqlCmd.Parameters.AddWithValue(p.Key, p.Value);
                            }
                        }

                        using (var sqlDa = new SqlDataAdapter(sqlCmd))
                        {
                            dsData = new DataSet();
                            sqlDa.Fill(dsData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }

            return dsData;
        }
    }
}
#endregion

using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GlobalCalendar.DAL
{
    public class DBConnection
    {
        private readonly string ConnStr;

        public DBConnection()
        {
            Microsoft.Extensions.Configuration.IConfiguration configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();


            ConnStr = configuration.GetConnectionString("TBSConnStr");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnStr);
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public DataSet ExecuteReaderDataSet(string SP_Name, List<SqlParameter> _param = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand(SP_Name, conn);

                cmd.CommandType = CommandType.StoredProcedure;
                if (_param != null)
                    cmd.Parameters.AddRange(_param.ToArray());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            return ds;
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public DataTable ExecuteReader(string SP_Name, List<SqlParameter> _param = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand(SP_Name, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (_param != null)
                    cmd.Parameters.AddRange(_param.ToArray());
                conn.Open();
                dt.Load(cmd.ExecuteReader());
                conn.Close();
            }
            return dt;
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public long ExecuteScalar(string SpName, List<SqlParameter> _param = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            long res = 0;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand(SpName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (_param != null)
                    cmd.Parameters.AddRange(_param.ToArray());
                conn.Open();
                res = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }
            return res;
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public int ExecuteNonQuery(string SpName, List<SqlParameter> _param = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            int res = 0;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand(SpName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (_param != null)
                    cmd.Parameters.AddRange(_param.ToArray());
                conn.Open();
                res = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return res;
        }
    }
}
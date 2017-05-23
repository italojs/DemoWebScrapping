using System.Data;
using System.Data.SqlClient;

namespace DemoWebScrapping2.DAL
{
    public class SqlServer : Database
    {
        public SqlServer()
        {
            SetDbConnection();
            SetBasicDbCommand();
        }

        public override void SetBasicDbCommand()
        {
            command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
        }

        public override void SetDbConnection()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Server=(LocalDB)\MSSQLLocalDB;Database=DB_WebScrapping;";
            connection = conn;
        }
    }
}

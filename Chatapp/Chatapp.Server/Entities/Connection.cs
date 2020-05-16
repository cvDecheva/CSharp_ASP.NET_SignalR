using System.Data.SqlClient;

namespace Chatapp.Server.Entities
{
    public static class Connection
    {
        public static SqlConnection Conn => privateConn;

        public static void Open(string conn)
        {
            privateConn = new SqlConnection();
            privateConn.ConnectionString = conn;
            privateConn.Open();
        }

        public static void Close()
        {
            privateConn?.Close();
        }

        #region Private members
        private static SqlConnection privateConn;
        #endregion
    }
}
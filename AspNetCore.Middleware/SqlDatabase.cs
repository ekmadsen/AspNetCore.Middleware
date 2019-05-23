using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public class SqlDatabase : IDatabase
    {
        private readonly string _connection;


        public SqlDatabase(string Connection)
        {
            _connection = Connection;
        }


        public async Task<IDbConnection> OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connection);
            await connection.OpenAsync();
            return connection;
        }
    }
}
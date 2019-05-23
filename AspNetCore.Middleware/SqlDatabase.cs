using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ErikTheCoder.AspNetCore.Middleware.Settings;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware
{
    [UsedImplicitly]
    public class SqlDatabase : IDatabase
    {
        private readonly string _connection;


        public SqlDatabase(IAppSettings AppSettings)
        {
            _connection = AppSettings.Database;
        }


        public async Task<DbConnection> OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connection);
            await connection.OpenAsync();
            return connection;
        }
    }
}
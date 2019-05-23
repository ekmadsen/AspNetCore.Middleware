using System.Data.Common;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public interface IDatabase
    {
        [UsedImplicitly]
        Task<DbConnection> OpenConnection();
    }
}
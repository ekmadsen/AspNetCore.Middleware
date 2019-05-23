using System.Data;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public interface IDatabase
    {
        [UsedImplicitly]
        Task<IDbConnection> OpenConnection();
    }
}
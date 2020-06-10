using System;
using System.Threading.Tasks;

namespace ContentApp.KeyVault
{
    public interface IKeyVaultPiece
    {
        Task<string> GetBlobConnectionString();
        Task<string> GetSqlConnectionString();
    }
}

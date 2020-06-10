using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace ContentApp.KeyVault
{
    public class KeyVaultPiece : IKeyVaultPiece
    {
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;
        private readonly KeyVaultClient keyVaultClient;
        public KeyVaultPiece()
        {
            this.azureServiceTokenProvider = new AzureServiceTokenProvider();

            this.keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(this.azureServiceTokenProvider.KeyVaultTokenCallback));

        }

        public async Task<string> GetSqlConnectionString()
        {
            string connectionString = string.Empty;
            try
            {
                var secret = await keyVaultClient.GetSecretAsync(
                    "https://cloudresource2secrets.vault.azure.net/secrets/SqlConnectionString")
                    .ConfigureAwait(false);
                connectionString = secret?.Value ?? string.Empty;
            }
            catch (Exception exp)
            {
            }

            return connectionString;
        }
        public async Task<string> GetBlobConnectionString()
        {
            string connectionString = string.Empty;
            try
            {
                var secret = await keyVaultClient.GetSecretAsync(
                    "https://cloudresource2secrets.vault.azure.net/secrets/BlobStorageAccountConnectionString")
                    .ConfigureAwait(false);
                connectionString = secret?.Value ?? string.Empty;
            }
            catch (Exception exp)
            {
            }

            return connectionString;
        }
    }
}

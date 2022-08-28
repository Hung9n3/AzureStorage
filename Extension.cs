using Azure.Storage.Blobs;
using AzureStorage.AppSetting;
using Microsoft.Extensions.Options;

namespace AzureStorage
{
    public class Extension
    {
        private IConfiguration _configuration;
        private IServiceProvider _provider;
        public Extension(IConfiguration configuration, IServiceProvider provider)
        {
            this._provider = provider;
            this._configuration = configuration;
        }
        public BlobContainerClient getContainer(string containerName)
        {
            var AzureStorageConnectionString = _configuration.GetConnectionString("AzureStorageConnectionString");
            var config = _provider.GetRequiredService<IOptionsMonitor<AzureStorageConfig>>().CurrentValue;
            return new BlobContainerClient(config.ConnectionString, containerName);
        }
    }
}

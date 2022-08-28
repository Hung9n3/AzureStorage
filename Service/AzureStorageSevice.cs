using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorage.AppSetting;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace AzureStorage.Service
{
    public class AzureStorageSevice
    {
        private readonly IOptionsMonitor<AzureStorageConfig> _storageConfig;
        private StorageSharedKeyCredential _storageCredentials;
        private BlobContainerClient _blobContainerClient;
        private IConfiguration _config;
        private BlobServiceClient _blobServiceClient;
        public AzureStorageSevice(IOptionsMonitor<AzureStorageConfig> azureStorageConfig,
                                            StorageSharedKeyCredential storageCredentials,
                                            Func<string, BlobContainerClient> blobContainerClient,
                                            Func<string, BlobServiceClient> blobServiceClitent,
                                            IConfiguration config)
        {
            this._storageCredentials = storageCredentials;
            this._storageConfig = azureStorageConfig;
            this._blobContainerClient = blobContainerClient("hung");
            this._blobServiceClient = blobServiceClitent("");
            _config = config;
        }
        public async Task<Tuple<string,string>> Upload(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            var blobUri = new Uri("https://" +
                                  _storageConfig.CurrentValue.AccountName +
                                  ".blob.core.windows.net/"+ containerName +
                                  "/" + file.FileName);
            // Create the blob client.
            var blobClient = new BlobClient(blobUri, _storageCredentials);
            
            // Upload the file
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });
            return new Tuple<string, string>(blobClient.Name, blobClient.Uri.AbsoluteUri);
        }
        public async Task Delete(string fileName, string containerName)
        {
            var blobClients = new BlobContainerClient(_config.GetConnectionString("AzureStorageConnectionString"), containerName);
            var blobClient =  blobClients.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
        public async Task<List<BlobContainerItem>> GetContainers()
        {
            List<BlobContainerItem> list = new List<BlobContainerItem>();
            var pageable = _blobServiceClient.GetBlobContainersAsync().AsPages(default);
            await foreach (Azure.Page<BlobContainerItem> containerPage in pageable)
            {
                foreach (BlobContainerItem containerItem in containerPage.Values)
                {
                    Console.WriteLine("Container name: {0}", containerItem.Name);
                    list.Add(containerItem);
                }

                Console.WriteLine();
            }
            return list;
        }
        public async Task<BlobContainerClient> AddContainers(string name)
        {
            BlobContainerClient containerClient = await _blobServiceClient.CreateBlobContainerAsync(name, PublicAccessType.BlobContainer);
            return containerClient;
        }
    }
}

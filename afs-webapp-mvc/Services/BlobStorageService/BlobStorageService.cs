using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace afs_webapp_mvc.Services.BlobStorageService
{
    public class BlobStorageService
    {
        private BlobServiceClient blobServiceClient;
        private readonly string _containerName;
        private readonly string _connectionString;

        public BlobStorageService(string connectionString, string containerName) 
        {
            this._connectionString = connectionString;
            this._containerName = containerName;
        }


        public async Task UploadBlobAsync(string blobID, Stream blobContentSream)
        {
            blobServiceClient = new BlobServiceClient(this._connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(this._containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobID);

            await blobClient.UploadAsync(blobContentSream, true);

        }

        public async Task<bool> IsExistAsync(string blobID)
        {
            blobServiceClient = new BlobServiceClient(this._connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(this._containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobID);

            return await blobClient.ExistsAsync();
        }

        public async Task<Stream> DownloadBlobAsync(string blobID)
        {
            blobServiceClient = new BlobServiceClient(this._connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(this._containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobID);

            var responseStream = await blobClient.OpenReadAsync();
            using(var memoryStream = new MemoryStream())
            {
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream;
            }
        }

        public async Task<string> GetBlobContentAsync(string blobID)
        {
            blobServiceClient = new BlobServiceClient(this._connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(this._containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobID);

            Stream response = await blobClient.OpenReadAsync();
            using (var reader = new StreamReader(response))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task DeleteBlobAsync(string blobID)
        {
            blobServiceClient = new BlobServiceClient(this._connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(this._containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobID);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}

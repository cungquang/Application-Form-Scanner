using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace BCHousing.AfsWebAppMvc.Servives.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _fileContainerClient;
        private readonly BlobContainerClient _stagingContainerClient;
        private readonly string _connectionString;
        private readonly string _fileContainerName = "file-container";
        private readonly string _stagingContainerName = "staging-container";

        public BlobStorageService(string connectionString)
        {
            _connectionString = connectionString;
            _blobServiceClient = new BlobServiceClient(_connectionString);
            _fileContainerClient = _blobServiceClient.GetBlobContainerClient(_fileContainerName);
            _stagingContainerClient = _blobServiceClient.GetBlobContainerClient(_stagingContainerName);
        }

        public async Task<Boolean> IsExistAsync(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            return await blobContainerClient.GetBlockBlobClient(blobName).ExistsAsync();
        }

        public async Task<bool> UploadBlobToAsync(string containerName, string blobName, Stream blobContent)
        {
            try
            {
                if (blobContent == null)
                {
                    throw new ArgumentException("Input data cannot be null", nameof(blobContent));
                }

                BlobClient blobClient = _stagingContainerClient.GetBlobClient(blobName);

                // Upload the new blob to Azure
                await blobClient.UploadAsync(blobContent, true);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> DownloadBlobFromAsync(string containerName, string blobName)
        {
            BlobClient blobClient = _fileContainerClient.GetBlobClient(blobName);

            var responseStream = await blobClient.OpenReadAsync();
            using (var memoryStream = new MemoryStream())
            {
                await responseStream.CopyToAsync(memoryStream);
                return memoryStream;
            }
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            try
            {
                // Validate if the file is existed
                if (!await IsExistAsync(containerName, blobName))
                {
                    throw new ArgumentException("Input container or blob does not exist", containerName + "/" + blobName);
                }

                BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainer.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CopyBlobToAsync(string sourceBlobName, string destinationContainer, string destinationBlobName)
        {
            // Validate if the file is existed
            if (!await IsExistAsync(_stagingContainerName, sourceBlobName))
            {
                throw new ArgumentException("Input container or blob does not exist", _stagingContainerName + "/" + sourceBlobName);
            }

            //Setup resource for source
            BlobClient blobSourceClient = _stagingContainerClient.GetBlobClient(sourceBlobName);

            //Setup resources for destination
            BlobContainerClient blobContainerDestinationClient = _blobServiceClient.GetBlobContainerClient(destinationContainer);
            BlobClient blobDestinationClient = blobContainerDestinationClient.GetBlobClient(destinationBlobName);

            // Write logic here
            return true;
        }

        public async Task<bool> WriteMetaDataAsync(string containerName, string blobName, string metadata)
        {
            // Validate if the file is existed
            if (!await IsExistAsync(containerName, blobName))
            {
                throw new ArgumentException("Input container or blob does not exist", containerName + "/" + blobName);
            }

            // Convert new metadata from json string to Dictionary
            if (string.IsNullOrEmpty(metadata))
            {
                throw new ArgumentException("Input cannot be null or empty.", nameof(metadata));
            }
            IDictionary<string, string>? newMetadata = JsonSerializer.Deserialize<IDictionary<string, string>>(metadata);

            // Get the current metadata
            BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainer.GetBlobClient(blobName);

            // Lease the blob
            //BlobLeaseClient leaseBlob = await AcquireBlobLeaseAsync(blobClient);

            try
            {
                var properties = await blobClient.GetPropertiesAsync();

                //Write/overwrite new metadata to the current metadata
                foreach(var pair in newMetadata)
                {
                    properties.Value.Metadata[pair.Key] = pair.Value;
                }

                //Update metadata
                await blobClient.SetMetadataAsync(properties.Value.Metadata);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IDictionary<string, string>> GetMetaDataAsync(string containerName, string blobName)
        {
            try
            {
                // Validate if the file is existed
                if (!await IsExistAsync(containerName, blobName))
                {
                    throw new ArgumentException("Input container or blob does not exist", containerName + "/" + blobName);
                }

                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                //Get properties of the blob: SubmitBy, DocumentName, UserDeclareType
                var blobProperties = await blobClient.GetPropertiesAsync();

                IDictionary<string, string> response = blobProperties.Value.Metadata;

                //Add SubmissionID (blob name), DocumentSize, Timestamp, URL
                response["SubmissionID"] = blobClient.Uri.Segments[^1];
                response["DocumentSize"] = blobProperties.Value.ContentLength.ToString();
                response["Timestamp"] = blobProperties.Value.LastModified.ToString("G");
                response["URL"] = blobClient.Uri.ToString();

                return response;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Stream> GetBlobContentAsync(string containerName, string blobName)
        {
            try
            {
                // Validate if the file is existed
                if (!await IsExistAsync(containerName, blobName))
                {
                    throw new ArgumentException("Input container or blob does not exist", containerName + "/" + blobName);
                }

                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                Stream response = await blobClient.OpenReadAsync();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////// Private Method //////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private static async Task<BlobLeaseClient> AcquireBlobLeaseAsync(BlobClient blobClientToLease)
        {
            try
            {
                BlobLeaseClient leaseClient = blobClientToLease.GetBlobLeaseClient();
                Response<BlobLease> response = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(20));
                return response == null ? throw new RequestFailedException("Failed to acquire lease on the blob") : leaseClient;
            }
            catch (Exception) 
            {
                throw;
            }
        }

        private static async Task RenewBlobLeaseAsync(BlobClient blobClientToLease, string leaseID)
        {
            BlobLeaseClient leaseClient = blobClientToLease.GetBlobLeaseClient(leaseID);

            await leaseClient.RenewAsync();
        }

        private static async Task ReleaseBlobLeaseAsync(BlobClient blobClientToLease, string leaseID)
        {
            BlobLeaseClient leaseClient = blobClientToLease.GetBlobLeaseClient(leaseID);

            await leaseClient.ReleaseAsync();
        }
    }
}

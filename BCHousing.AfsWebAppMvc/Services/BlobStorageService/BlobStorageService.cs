using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.IdentityModel.Tokens;

namespace BCHousing.AfsWebAppMvc.Servives.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _connectionString;

        public BlobStorageService(string connectionString)
        {
            _connectionString = connectionString;
            _blobServiceClient = new BlobServiceClient(_connectionString);
        }

        public async Task<Boolean> IsExistAsync(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            return await blobContainerClient.GetBlockBlobClient(blobName).ExistsAsync();
        }

        public async Task<string> UploadBlobToAsync(string containerName, string blobName, Stream blobContent, [Optional] string metadata)
        {
            try
            {
                if (blobContent == null)
                {
                    throw new ArgumentException("Input data cannot be null", nameof(blobContent));
                }

                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                // Upload the new blob to Azure
                if (!string.IsNullOrEmpty(metadata))
                {
                    IDictionary<string, string>? newMetadata = JsonSerializer.Deserialize<IDictionary<string, string>>(metadata);
                    await blobClient.UploadAsync(blobContent, new BlobUploadOptions {
                        Metadata = newMetadata
                    });
                }
                else
                {
                    await blobClient.UploadAsync(blobContent, true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Stream> DownloadBlobFromAsync(string containerName, string blobName)
        {
            // Validate if the file is existed
            if (!await IsExistAsync(containerName, blobName))
            {
                throw new ArgumentException("Input container or blob does not exist", containerName + "/" + blobName);
            }

            try
            {
                BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainer.GetBlobClient(blobName);

                var responseStream = new MemoryStream();
                await blobClient.DownloadToAsync(responseStream);
                responseStream.Position = 0;
                return responseStream;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            try
            {
                BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainer.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> GetBlobContentAsync(string containerName, string blobName)
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

                using var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);

                return memoryStream.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////// API Method ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task<string> CopyBlobToAsync(string sourceContainer, string sourceBlobName, string destinationContainer, string destinationBlobName)
        {
            try
            {
                // Validate if the file is existed
                if (!await IsExistAsync(sourceContainer, sourceBlobName))
                {
                    throw new ArgumentException("Input container or blob does not exist", sourceContainer + "/" + sourceBlobName);
                }
            
                //Get content from source
                var sourceContent = await DownloadBlobFromAsync(sourceContainer, sourceBlobName);

                //Create and upload content to destination
                string uri = await UploadBlobToAsync(destinationContainer, destinationBlobName, sourceContent);

                //Delete source blob
                await DeleteBlobAsync(sourceContainer, sourceBlobName);

                return uri;
            }
            catch(Exception) {
                throw;
            }
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

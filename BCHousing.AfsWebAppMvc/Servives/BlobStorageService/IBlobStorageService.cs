namespace BCHousing.AfsWebAppMvc.Servives.BlobStorageService
{
    public interface IBlobStorageService
    {
        public Task<Boolean> IsExistAsync(string containerName, string blobName);

        public Task<bool> UploadBlobToAsync(string containerName, string blobName, Stream blobContent);

        public Task<Stream> DownloadBlobFromAsync(string containerName, string blobName);

        public Task DeleteBlobAsync(string containerName, string blobName);

        public Task<bool> CopyBlobToAsync(string sourceContainer, string sourceBlobName, string destinationContainer, string destinationBlobName);

        public Task<bool> WriteMetaDataAsync(string containerName, string blobName, string metadata);

        public Task<IDictionary<string, string>> GetMetaDataAsync(string containerName, string blobName);

        public Task<Stream> GetBlobContentAsync(string containerName, string blobName);
    }
}

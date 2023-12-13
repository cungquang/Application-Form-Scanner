using System.Runtime.InteropServices;

namespace BCHousing.AfsWebAppMvc.Entities.BlobStorage
{
    public class UploadBlobPostRequest
    {
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public string BlobContent { get; set; }
        public string Metadata { get; set; }
    }
}

namespace BCHousing.AfsWebAppMvc.Entities.BlobStorage
{
    public class CopyBlobPostRequest
    {
        public string SourceURL { get; set; }
        public string DestinationContainer {  get; set; }
        public string? DestinationFolder { get; set; }   
    }
}

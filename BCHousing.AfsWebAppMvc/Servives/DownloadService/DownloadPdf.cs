namespace BCHousing.AfsWebAppMvc.Servives.DownloadService
{
    public class DownloadPdf : IDownloadService<DownloadPdf>
    {
        public async Task<byte[]> DownloadDocumentAsync(string documentID)
        {
            await Task.Delay(100);
            return default;
        }
    }
}

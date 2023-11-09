namespace afs_webapp_mvc.Services.DownloadService
{
    public class DownloadPdf : IDownload
    {
        public async Task<byte[]> DownloadDocumentAsync(string documentID)
        {
            await Task.Delay(1000);
            return null;
        }
    }
}

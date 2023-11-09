namespace afs_webapp_mvc.Services.DownloadService
{
    public class DownloadService
    {
        private readonly List<IDownload> _downloadEngine;

        public DownloadService(List<IDownload> downloadEngine)
        {
            _downloadEngine = downloadEngine;
        }

        public async Task<byte[]?> StartDownload(string downloadType, string documentID)
        {
            switch (downloadType.ToLower())
            {
                case "pdf":
                    return await _downloadEngine[0].DownloadDocumentAsync(documentID);
                case "excel":
                    return await _downloadEngine[1].DownloadDocumentAsync(documentID);
            }

            return default;
        }
    }
}

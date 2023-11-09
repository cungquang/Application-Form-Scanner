namespace afs_webapp_mvc.Services.DownloadService
{
    public interface IDownload
    {
        /*
         * DownloadAsync
         * Description: 
         *  -> Download document per request type (Ex: web view, pdf, excel...), and return data in byte
         * Input:
         *  -> string documentID: identification of the document
         * Output:
         *  -> return the document in byte
         */
        Task<byte[]> DownloadDocumentAsync(string documentID);
    }
}

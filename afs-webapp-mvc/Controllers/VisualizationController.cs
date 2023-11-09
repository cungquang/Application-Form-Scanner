using afs_webapp_mvc.Models;
using afs_webapp_mvc.Services.DownloadService;
using Microsoft.AspNetCore.Mvc;

namespace afs_webapp_mvc.Controllers
{
    public class VisualizationController : Controller
    {
        private readonly IDownload _downloadService;

        public VisualizationController(IDownload downloadService)
        {
            _downloadService = downloadService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DownloadDocument(long documentID)
        {
            var pdf = await _downloadService.DownloadDocumentAsync("testID");
            return File(pdf, "application/pdf", "testDocument.pdf");
        }
    }
}

using afs_webapp_mvc.Models;
using afs_webapp_mvc.Services.DownloadService;
using Microsoft.AspNetCore.Mvc;

namespace afs_webapp_mvc.Controllers
{
    public class VisualizationController : Controller
    {
        private readonly IDownloadService _downloadPdf;

        public VisualizationController(IDownloadService downloadPdf)
        {
            _downloadPdf = downloadPdf;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DownloadDocument(long documentID)
        {
            var pdf = await _downloadPdf.DownloadDocumentAsync("testID");
            return File(pdf, "application/pdf", "testDocument.pdf");
        }
    }
}

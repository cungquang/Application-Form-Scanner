using BCHousing.AfsWebAppMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BCHousing.AfsWebAppMvc.Controllers
{
    public class VisualizationController : Controller
    {
        //private readonly IDownloadService _downloadPdf;

        /*public VisualizationController(IDownloadService downloadPdf)
        {
            _downloadPdf = downloadPdf;
        }*/

        public VisualizationController() { }
        public IActionResult Index()
        {
            return View();
        }

        /*public async Task<IActionResult> DownloadDocument(long documentID)
        {
            var pdf = await _downloadPdf.DownloadDocumentAsync("testID");
            return File(pdf, "application/pdf", "testDocument.pdf");
        }*/

        public async Task<IActionResult> ViewDocument(long documentID)
        {
            await Task.Delay(1000);
            return View();
        }

        public async Task<IActionResult> EditDocument(long documentID)
        {
            await Task.Delay(1000);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

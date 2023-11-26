using BCHousing.AfsWebAppMvc.Models;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using BCHousing.AfsWebAppMvc.Servives.SessionManagementService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BCHousing.AfsWebAppMvc.Controllers
{
    public class VisualizationController : Controller
    {
        private readonly SessionManagementService _sessionManagementService;
        private readonly ILogger<HomeController> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public VisualizationController(SessionManagementService sessionManagementService, ILogger<HomeController> logger, IBlobStorageService blobStorageService)
        {
            _sessionManagementService = sessionManagementService;
            _logger = logger;
            _blobStorageService = blobStorageService;
        }
        public IActionResult Index()
        {
            var model = _sessionManagementService.GetUploadFileViewModel();
            ViewBag.UserName = $"Hello {model.FirstName}, {model.LastName}";
            ViewBag.FileUpload = $"Has upload the file {model.UploadFileName} with size = {model.UploadFileSize}";
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

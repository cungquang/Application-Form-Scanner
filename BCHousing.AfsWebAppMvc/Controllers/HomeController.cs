using BCHousing.AfsWebAppMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BCHousing.AfsWebAppMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new UploadFileViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Submit(UploadFileViewModel model)
        {
            return Content($"Form: Name={model.FirstName} {model.LastName} - Document Type={model.DocumentType} - Document: {model.UploadFile.FileName}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Models;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using BCHousing.AfsWebAppMvc.Servives.SessionManagementService;
using BCHousing.AfsWebAppMvc.Servives.UtilityService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace BCHousing.AfsWebAppMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly SessionManagementService _sessionManagementService;
        private readonly ILogger<HomeController> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public HomeController(
            SessionManagementService sessionManagementService, 
            ILogger<HomeController> logger, 
            IBlobStorageService blobStorageService
        )
        {
            _sessionManagementService = sessionManagementService;
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.Client, NoStore = true)]
        public IActionResult Submission()
        {
            try
            {
                var model = new UploadFileViewModel();
                ViewBag.Back = "Index";
                return View(model);
            }
            catch (Exception ex)
            {
                LogRequestError(ex);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submission(UploadFileViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string blobName = UtilityService.GenerateSystemGuid().ToString() + $".{model.UploadFile.FileName.Split(".")[^1]}";

                    //Upload new blob
                    if(await _blobStorageService.UploadBlobToAsync("staging-container", blobName, model.UploadFile.OpenReadStream()))
                    {
                        //Write metadata to the blob
                        await _blobStorageService.WriteMetaDataAsync("staging-container", blobName, await UtilityService.SerializeMetadataAsync(model));
                    }

                    // Refresh the page
                    return RedirectToAction(nameof(HomeController.Submission));
                }

                // Need to return to the page with current data - in case invalid data
                return View(model);

            }
            catch(Exception ex)
            {
                LogRequestError(ex);
                throw;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private void LogRequestError(Exception ex)
        {
            _logger.LogError($"An Exception Occured while processing the request with ID: {Activity.Current?.Id ?? HttpContext.TraceIdentifier}. Message: {ex.Message}");
        }
    }
}
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
            var model = _sessionManagementService.GetSubmissionViewInputModel();
            return View(model);
        }

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

        public IActionResult Visualization()
        {
            var model = new ListOfFilesVisualizationViewModel()
            {
                NumberOfFile = 50
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submission(UploadFileViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Create Session
                    string blobName = UtilityService.GenerateSystemGuid().ToString() + $".{model.UploadFile.FileName.Split(".")[^1]}";
                    string newBlobURL = await _blobStorageService.UploadBlobToAsync(
                        "staging-container", 
                        blobName, model.UploadFile.OpenReadStream(), 
                        await UtilityService.SerializeMetadataAsync(model)
                        );
                    
                    // Verify the upload
                    if (!string.IsNullOrEmpty(newBlobURL))
                    { 
                        _sessionManagementService.SetSubmissionViewInputModel(model, true);
                    }
                    else
                    {
                        _sessionManagementService.SetSubmissionViewInputModel(model, false);
                    }

                    // Refresh the page after submit
                    return RedirectToAction(nameof(HomeController.Submission));
                }

                // return to the page with current data - trigger validation
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
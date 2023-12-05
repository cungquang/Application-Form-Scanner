using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Models;
using BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using BCHousing.AfsWebAppMvc.Servives.CacheManagementService;
using BCHousing.AfsWebAppMvc.Servives.SessionManagementService;
using BCHousing.AfsWebAppMvc.Servives.UtilityService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Web;

namespace BCHousing.AfsWebAppMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly CacheManagementService _cacheManagementService;
        private readonly SessionManagementService _sessionManagementService;
        private readonly ILogger<HomeController> _logger;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IAfsDatabaseService _afsDatabaseService;

        public HomeController(
            CacheManagementService cacheManagementService,
            SessionManagementService sessionManagementService, 
            IAfsDatabaseService afsDatabaseService,
            ILogger<HomeController> logger, 
            IBlobStorageService blobStorageService
        )
        {
            _cacheManagementService = cacheManagementService;
            _sessionManagementService = sessionManagementService;
            _blobStorageService = blobStorageService;
            _afsDatabaseService = afsDatabaseService;
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            await _cacheManagementService.RefreshCacheAsync(CacheKey.GetSubmissionLogCacheKey(), () => _afsDatabaseService.GetAllSubmissionLogsSync());
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

        public async Task<IActionResult> Visualization()
        {
            try
            {
                Task<IList<SubmissionLog>>? CacheData = _cacheManagementService.GetCachedDataAsync(
                                    CacheKey.GetSubmissionLogCacheKey(),
                                    async () => await _afsDatabaseService.GetAllSubmissionLogsSync()
                                );
                var model = new SubmissionLogsVisualizationViewModel(await CacheData);
                return View(model);
            }
            catch(Exception ex)
            {
                LogRequestError(ex);
                throw;
            }
        }

        public async Task<IActionResult> Edit(string submissionId, string classifyType, int? activeTab)
        {
            IList<Form> formFields = await _afsDatabaseService.GetFormBySubmissionIdAsync(UtilityService.ConvertStringToGuid(submissionId));
            var model = new FormsEditViewModel(submissionId, classifyType.Trim(), formFields);
            ViewBag.ActiveTab = activeTab ?? 1;
            TempData["ClassifyType"] = classifyType.Trim();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string submissionId, int? activeTab, IList<Form> model)
        {
            if(ModelState.IsValid)
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var submissionFields = await _afsDatabaseService.SetFormBySubmissionIdAndSequenceAsync(UtilityService.ConvertStringToGuid(submissionId), model[i].sequence ?? 1, model[i].field_value);
                }
                return RedirectToAction("Edit", new { submissionId, classifyType = TempData["ClassifyType"], activeTab });
            }

            return RedirectToAction(nameof(Visualization));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(string url)
        {
            Dictionary<string, string> UrlParts = await UtilityService.GetContainerAndBlobName(url);
            string blobFullPath = string.IsNullOrEmpty(UrlParts["Folder Name"]) ? UrlParts["Blob Name"] : $"{UrlParts["Folder Name"]}/{UrlParts["Blob Name"]}";

            var pdfStream = await _blobStorageService.DownloadBlobFromAsync(UrlParts["Container Name"], blobFullPath);
            
            return File(pdfStream, "application/pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Download(string url)
        {
            try
            {
                Dictionary<string, string> UrlParts = await UtilityService.GetContainerAndBlobName(url);
                string blobFullPath = string.IsNullOrEmpty(UrlParts["Folder Name"]) ? UrlParts["Blob Name"] : $"{UrlParts["Folder Name"]}/{UrlParts["Blob Name"]}";
                var fileStream = await _blobStorageService.DownloadBlobFromAsync(UrlParts["Container Name"], blobFullPath);
                var fileName = $"{UrlParts["Blob Name"].Split(".")[0]}-{DateTime.UtcNow:yyyyMMdd}.pdf";
                var contentType = "application/pdf";

                return File(fileStream, contentType, fileName);
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
                    // Create Session
                    string blobName = UtilityService.GenerateSystemGuid().ToString() + $".{model.UploadFile.FileName.Split(".")[^1]}";
                    string newBlobURL = await _blobStorageService.UploadBlobToAsync(
                        "staging-container", 
                        blobName, model.UploadFile.OpenReadStream(), 
                        await UtilityService.SerializeMetadataAsync(model)
                        );
                    
                    // Verify the upload - update the session with new data
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

        public async Task<IActionResult> Refresh()
        {
            await _cacheManagementService.RefreshCacheAsync(CacheKey.GetSubmissionLogCacheKey(),
                async () => await _afsDatabaseService.GetAllSubmissionLogsSync());
            return RedirectToAction("Visualization");
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
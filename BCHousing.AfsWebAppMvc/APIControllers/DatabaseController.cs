using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BCHousing.AfsWebAppMvc.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IAfsDatabaseService _afsDatabaseService;
        private readonly IBlobStorageService _blobStorageService;

        public DatabaseController(IAfsDatabaseService afsDatabaseService, IBlobStorageService blobStorageService)
        {
            _afsDatabaseService = afsDatabaseService;
            _blobStorageService = blobStorageService;
        }

        // GET: api/Database/GetSubmissionLogs
        [HttpGet("GetSubmissionLogs")]
        public async Task<IList<SubmissionLog>> GetSubmissionLogsAsync()
        {
            return await _afsDatabaseService.GetAllSubmissionLogsAsync();
        }

        // GET: api/DataBase/GetSubmissionLogByUrl?fileUrl=<file Url>
        [HttpGet("GetSubmissionLogByUrl")]
        public async Task<SubmissionLog> GetSubmissionLogByUrlAsync([FromQuery] string fileUrl)
        {
            return await _afsDatabaseService.GetSubmissionLogByUrlAsync(fileUrl);
        }


        // POST: api/Database/CreateSubmissionLog
        [HttpPost("CreateSubmissionLog")]
        public async Task<string> CreateSubmissionLogAsync([FromBody] SubmissionLogRequest requestBody)
        {
            // Blob API will return a proper metadata
            string[] urlInformation = requestBody.FileUrl.Split("/");
            string filename = urlInformation[^1];
            string container = urlInformation[^2];

            IDictionary<string,string> blobInfo = await _blobStorageService.GetMetaDataAsync(container, filename);
            string blobSubmissionId = blobInfo["SubmissionID"].Split("/")[^1].Split(".")[0];

            SubmissionLog newLog = new SubmissionLog()
            {
                submissionId = Guid.Parse(blobSubmissionId),
                timestamp = DateTime.Parse(blobInfo["Timestamp"]),
                submit_by = blobInfo["SubmitBy"],
                document_name = blobInfo["DocumentName"],
                document_size = int.Parse(blobInfo["DocumentSize"]),
                user_declared_type = blobInfo["UserDeclaredType"],
                classify_type = requestBody.ClassifyType,
                is_read = false,
                path_to_document = requestBody.FileUrl
            };


            return await _afsDatabaseService.CreateSubmissionLogAsync(newLog);
        }

        // POST: api/Database/CreateFormRecord
        [HttpPost("CreateFormRecord")]
        public async Task<FormRecordRequest> CreateFormRecordAsync([FromBody] FormRecordRequest requestBody)
        { 
            return await _afsDatabaseService.CreateFormRecordAsync(requestBody);
        }

        // PUT: api/Database/UpdatePathToFile
        [HttpPut("UpdatePathToFile")]
        public async Task<string> UpdatePathToFileAsync([FromBody] UpdateFilePath requestBody) {
            return await _afsDatabaseService.UpdatePathToFileAsync(requestBody);
        }

        // PUT: api/Database/UpdateSubmissionLogAfterOCRExtraction
        [HttpPut("UpdateSubmissionLogAfterOCRExtraction")]
        public async Task<string> UpdateSubmissionLogAfterOCRExtractionAsync([FromBody] UpdateLogAfterOCRExtraction requestBody)
        {
            return await _afsDatabaseService.UpdateSubmissionLogAfterOCRExtractionAsync(requestBody);
        }
    }
}

using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.BlobStorage;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Repositories;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32.SafeHandles;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BCHousing.AfsWebAppMvc.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {

        private readonly ISubmissionLogRepository _submissionLogRepository;
        private readonly IFormRepository _formRepository;
        private readonly IBlobStorageService _blobStorageService;

        public DatabaseController(ISubmissionLogRepository submissionLogRepository, IFormRepository formRepository, IBlobStorageService blobStorageService)
        {
            _submissionLogRepository = submissionLogRepository;
            _formRepository = formRepository;
            _blobStorageService = blobStorageService;
        }

        // GET: api/Database/GetSubmissionLogs
        [HttpGet("GetSubmissionLogs")]
        public async Task<IList<SubmissionLog>> GetSubmissionLogs()
        {
            return await _submissionLogRepository.GetSubmissionLogs();
        }

        // GET: api/DataBase/GetSubmissionLogByUrl?fileUrl=<file Url>
        [HttpGet("GetSubmissionLogByUrl")]
        public async Task<SubmissionLog> GetSubmissionLogByUrl([FromQuery] string fileUrl)
        {
            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLog(fileUrl);

            return submissionLog;
        }


        // POST: api/Database/CreateSubmissionLog
        [HttpPost("CreateSubmissionLog")]
        public async Task<string> CreateSubmissionLog([FromBody] SubmissionLogRequest requestBody)
        {
            // TODO: Blob API will return a proper metadata
            string[] urlInformation = requestBody.FileUrl.Split("/");
            string filename = urlInformation[urlInformation.Length - 1];
            string container = urlInformation[urlInformation.Length - 2];

            System.Diagnostics.Debug.WriteLine($"filename: {filename}");
            System.Diagnostics.Debug.WriteLine($"container: {container}");

            var blobInfo = await _blobStorageService.GetMetaDataAsync(container, filename);
            
            SubmissionLog newLog = new SubmissionLog();
            newLog.submissionId = Guid.NewGuid();
            newLog.timestamp = DateTime.Parse(blobInfo["Timestamp"]);
            newLog.submit_by = blobInfo["SubmitBy"];
            newLog.document_name = blobInfo["DocumentName"];
            newLog.document_size = int.Parse(blobInfo["DocumentSize"]);
            newLog.user_declared_type = blobInfo["UserDeclaredType"];
            newLog.classify_type = requestBody.ClassifyType;
            newLog.avg_confidence_level = (decimal)0.925; // TODO: this should be allow to be Null
            newLog.is_read = false;
            newLog.path_to_document = requestBody.FileUrl;
            newLog.path_to_analysis_report = requestBody.FileUrl; // TODO: this should be allow to be Null


            int result = await _submissionLogRepository.CreateSubmissionLog(newLog); 
            string resultMessage = "Insertion Failed";
            if (result == 1) {
                resultMessage = "Insertion Successfully";
            }

            return resultMessage;
        }

        // POST: api/Database/CreateFormRecord
        [HttpPost("CreateFormRecord")]
        public async Task<FormRecordRequest> CreateFormRecord([FromBody] FormRecordRequest requestBody)
        {
          List<FormData> formDatas = requestBody.FormDatas;

            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLog("https://afspocstorage.blob.core.windows.net/staging-container/RAP_training_data_03.pdf");
            Guid? submissionId = submissionLog.submissionId;

           foreach (FormData data in formDatas) {
                var form = new Form() {
                    submissionId = submissionId,
                    sequence = int.Parse(data.Key),
                    field_name = "Standby",
                    field_value = data.Content
                    
            };
                await _formRepository.CreateFormRecord(form);
            }

            return requestBody;
        }
    }
}

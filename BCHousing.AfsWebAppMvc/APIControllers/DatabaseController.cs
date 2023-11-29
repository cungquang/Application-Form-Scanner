﻿using BCHousing.AfsWebAppMvc.Entities;
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
            string filename = urlInformation[^1];
            string container = urlInformation[^2];

            System.Diagnostics.Debug.WriteLine($"filename: {filename}");
            System.Diagnostics.Debug.WriteLine($"container: {container}");

            var blobInfo = await _blobStorageService.GetMetaDataAsync(container, filename);
            string blobSubmissionId = blobInfo["SubmissionID"].Split("/")[^1].Split(".")[0];

            SubmissionLog newLog = new SubmissionLog();
            newLog.submissionId = Guid.Parse(blobSubmissionId);
            newLog.timestamp = DateTime.Parse(blobInfo["Timestamp"]);
            newLog.submit_by = blobInfo["SubmitBy"];
            newLog.document_name = blobInfo["DocumentName"];
            newLog.document_size = int.Parse(blobInfo["DocumentSize"]);
            newLog.user_declared_type = blobInfo["UserDeclaredType"];
            newLog.classify_type = requestBody.ClassifyType;
            newLog.is_read = false;
            newLog.path_to_document = requestBody.FileUrl;


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

            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLog(requestBody.fileUrl);
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

        // PUT: api/Database/UpdatePathToFile
        [HttpPut("UpdatePathToFile")]
        public async Task<string> UpdatePathToFile([FromBody] UpdateFilePath requestBody) {
            int result = await _submissionLogRepository.UpdatePathToFile(requestBody);
            string message = "Log path_to_file not update";
            if (result == 1) {
                message = "Log path_to_file updated";
            }

            return message;
        }

        // PUT: api/Database/UpdateSubmissionLogAfterOCRExtraction
        [HttpPut("UpdateSubmissionLogAfterOCRExtraction")]
        public async Task<string> UpdateSubmissionLogAfterOCRExtraction([FromBody] UpdateLogAfterOCRExtraction requestBody)
        {
            int result = await _submissionLogRepository.UpdateLogAfterExtractOCR(requestBody);
            string message = "Log (avg_confidence_score, isRead, path_to_analysis_report) not update";
            if (result == 1)
            {
                message = "Log (avg_confidence_score, isRead, path_to_analysis_report) updated";
            }

            return message;
        }
    }
}

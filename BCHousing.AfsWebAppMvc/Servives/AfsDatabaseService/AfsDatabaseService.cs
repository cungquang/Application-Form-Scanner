using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Repositories;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService
{
    public class AfsDatabaseService : IAfsDatabaseService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISubmissionLogRepository _submissionLogRepository;
        private readonly IFormRepository _formRepository;

        public AfsDatabaseService(IMemoryCache memoryCache, IBlobStorageService blobStorageService, IFormRepository formRepository, ISubmissionLogRepository submissionLogRepository)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _blobStorageService = blobStorageService;
            _submissionLogRepository = submissionLogRepository;
            _formRepository = formRepository;
        }

        public async Task<IList<SubmissionLog>?> GetAllSubmissionLogs()
        {
            // If data not in the cache -> retrieve data from database -> save to submissionLogCachedData
            if (!_memoryCache.TryGetValue(CacheKey.SubmissionLogKey, out IList<SubmissionLog>? submissionLogCachedData))
            {
                // Request data from Repository
                submissionLogCachedData = await _submissionLogRepository.GetSubmissionLogs();

                // Setup Cache option
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    // Cache will be fixed remove from the Cache after 15 minutes
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4),

                    // If no access to the data with 3 minutes -> remove from the cache -> need to retrieve data again
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                // Cache new data into memory
                _memoryCache.Set(CacheKey.SubmissionLogKey, submissionLogCachedData, cacheEntryOptions);
            }

            return submissionLogCachedData;
        }

        public async Task<IList<SubmissionLog>> GetAllSubmissionLogsSync()
        {
            return await _submissionLogRepository.GetSubmissionLogs();
        }

        public async Task<SubmissionLog> GetSubmissionLogByUrl(string fileUrl)
        {
            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLog(fileUrl);

            return submissionLog;
        }

        public async Task<string> CreateSubmissionLog(SubmissionLogRequest requestBody)
        {
            // Blob API will return a proper metadata
            string[] urlInformation = requestBody.FileUrl.Split("/");
            string filename = urlInformation[^1];
            string container = urlInformation[^2];

            var blobInfo = await _blobStorageService.GetMetaDataAsync(container, filename);
            string blobSubmissionId = blobInfo["SubmissionID"].Split("/")[^1].Split(".")[0];

            SubmissionLog newLog = new SubmissionLog() {
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
            
            int result = await _submissionLogRepository.CreateSubmissionLog(newLog);
            string resultMessage = "Insertion Failed";
            if (result == 1)
            {
                resultMessage = "Insertion Successfully";
            }

            return resultMessage;
        }

        public async Task<FormRecordRequest> CreateFormRecord(FormRecordRequest requestBody)
        {
            List<FormData> formDatas = requestBody.FormDatas;

            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLog(requestBody.fileUrl);
            Guid? submissionId = submissionLog.submissionId;

            foreach (FormData data in formDatas)
            {
                var form = new Form()
                {
                    submissionId = submissionId,
                    sequence = int.Parse(data.Key),
                    field_name = "Standby",
                    field_value = data.Content

                };
                await _formRepository.CreateFormRecord(form);
            }

            return requestBody;
        }

        public async Task<string> UpdatePathToFile(UpdateFilePath requestBody)
        {
            int result = await _submissionLogRepository.UpdatePathToFile(requestBody);
            string message = "Log path_to_file not update";
            if (result == 1)
            {
                message = "Log path_to_file updated";
            }

            return message;
        }

        public async Task<string> UpdateSubmissionLogAfterOCRExtraction(UpdateLogAfterOCRExtraction requestBody)
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

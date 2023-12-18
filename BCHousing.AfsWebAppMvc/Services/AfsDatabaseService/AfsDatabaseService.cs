using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Entities.FieldNames;
using BCHousing.AfsWebAppMvc.Repositories;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService
{
    public class AfsDatabaseService : IAfsDatabaseService
    {
        private readonly ISubmissionLogRepository _submissionLogRepository;
        private readonly IFormRepository _formRepository;

        public AfsDatabaseService(IBlobStorageService blobStorageService, IFormRepository formRepository, ISubmissionLogRepository submissionLogRepository)
        {
            _submissionLogRepository = submissionLogRepository;
            _formRepository = formRepository;
        }

        public async Task<IList<SubmissionLog>> GetAllSubmissionLogsAsync()
        {
            return await _submissionLogRepository.GetSubmissionLogsAsync();
        }

        public async Task<SubmissionLog> GetSubmissionLogByUrlAsync(string fileUrl)
        {
            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLogAsync(fileUrl);

            return submissionLog;
        }

        public async Task<string> CreateSubmissionLogAsync(SubmissionLog newLog)
        {   
            int result = await _submissionLogRepository.CreateSubmissionLogAsync(newLog);
            string resultMessage = "Insertion Failed";
            if (result == 1)
            {
                resultMessage = "Insertion Successfully";
            }

            return resultMessage;
        }

        public async Task<FormRecordRequest> CreateFormRecordAsync(FormRecordRequest requestBody, string fileType)
        {
            List<FormData> formDatas = requestBody.FormDatas;

            SubmissionLog submissionLog = await _submissionLogRepository.GetSubmissionLogAsync(requestBody.fileUrl);
            Guid? submissionId = submissionLog.submissionId;
            
            string fieldname = "Standby";
            foreach (FormData data in formDatas)
            {
                if (fileType.Equals("SAFER"))
                {
                    // SAFER
                    fieldname = SaferFieldNames.GetFieldNameBySequence(int.Parse(data.Key));
                }
                else 
                {
                    // RAP
                    fieldname = RapFieldNames.GetFieldNameBySequence(int.Parse(data.Key));
                }

                var form = new Form()
                {
                    submissionId = submissionId??UtilityService.UtilityService.GenerateSystemGuid(),
                    sequence = int.Parse(data.Key),
                    field_name = fieldname,
                    field_value = data.Content

                };
                await _formRepository.CreateFormRecordAsync(form);
            }

            return requestBody;
        }

        public async Task<string> UpdatePathToFileAsync(UpdateFilePath requestBody)
        {
            int result = await _submissionLogRepository.UpdatePathToFileAsync(requestBody);
            string message = "Log path_to_file not update";
            if (result == 1)
            {
                message = "Log path_to_file updated";
            }

            return message;
        }

        public async Task<string> UpdateSubmissionLogAfterOCRExtractionAsync(UpdateLogAfterOCRExtraction requestBody)
        {
            int result = await _submissionLogRepository.UpdateLogAfterExtractOCRAsync(requestBody);
            string message = "Log (avg_confidence_score, isRead, path_to_analysis_report) not update";
            if (result == 1)
            {
                message = "Log (avg_confidence_score, isRead, path_to_analysis_report) updated";
            }

            return message;
        }

        public async Task<IList<SubmissionLog>> GetAllSaferRapSubmissionLogAsync()
        {
            var submission = await _submissionLogRepository.GetSubmissionLogsAsync();
            return submission.Where(submission =>
                submission.classify_type.Trim().Equals("SAFER", StringComparison.OrdinalIgnoreCase) || 
                submission.classify_type.Trim().Equals("RAP", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<IList<Form>> GetFormBySubmissionIdAsync(Guid targetSubmissionId)
        {
            return await _formRepository.GetFormRecordAsync(targetSubmissionId);
        }

        public async Task<int> SetFormBySubmissionIdAndSequenceAsync(Guid targetSubmissionId, int sequence, string? newValue)
        {
            return (await _formRepository.UpdateFormBySubmissionIdAndSequenceAsync(targetSubmissionId, sequence, newValue));
        }

    }
}

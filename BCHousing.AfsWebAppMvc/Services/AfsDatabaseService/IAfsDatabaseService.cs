using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService
{
    public interface IAfsDatabaseService
    {
        public Task<IList<SubmissionLog>> GetAllSubmissionLogsAsync();

        public Task<SubmissionLog> GetSubmissionLogByUrlAsync(string fileUrl);

        public Task<string> CreateSubmissionLogAsync(SubmissionLog newLog);

        public Task<FormRecordRequest> CreateFormRecordAsync(FormRecordRequest requestBody, string fileType);

        public Task<string> UpdatePathToFileAsync(UpdateFilePath requestBody);

        public Task<string> UpdateSubmissionLogAfterOCRExtractionAsync(UpdateLogAfterOCRExtraction requestBody);

        public Task<IList<SubmissionLog>> GetAllSaferRapSubmissionLogAsync();

        public Task<IList<Form>> GetFormBySubmissionIdAsync(Guid targetSubmissionId);

        public Task<int> SetFormBySubmissionIdAndSequenceAsync(Guid targetSubmissionId, int sequence, string? newValue);

    }
}

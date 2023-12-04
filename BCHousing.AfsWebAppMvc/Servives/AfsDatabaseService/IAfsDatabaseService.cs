using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService
{
    public interface IAfsDatabaseService
    {
        public Task<IList<SubmissionLog>> GetAllSubmissionLogsSync();

        public Task<SubmissionLog> GetSubmissionLogByUrl(string fileUrl);

        public Task<string> CreateSubmissionLog(SubmissionLogRequest requestBody);

        public Task<FormRecordRequest> CreateFormRecord(FormRecordRequest requestBody);

        public Task<string> UpdatePathToFile(UpdateFilePath requestBody);

        public Task<string> UpdateSubmissionLogAfterOCRExtraction(UpdateLogAfterOCRExtraction requestBody);

        public Task<IList<SubmissionLog>> GetAllSaferRapSubmissionLogAsync();

        public Task<IList<Form>> GetFormBySubmissionIdAsync(Guid targetSubmissionId);

        public Task<int> SetFormBySubmissionIdAndSequenceAsync(Guid targetSubmissionId, int sequence, string? newValue);

    }
}

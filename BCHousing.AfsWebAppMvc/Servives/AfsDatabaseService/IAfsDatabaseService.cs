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

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////// Cache Method ////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public Task<IList<SubmissionLog>?> GetAllSubmissionLogsCacheAsync();

        public Task<IList<Form>?> GetFormRecordCacheAsync(string targetSubmissionId);
    }
}

using BCHousing.AfsWebAppMvc.Entities;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService
{
    public interface IAfsDatabaseService
    {
        public Task<IList<SubmissionLog>?> GetAllSubmissionLogs();
    }
}

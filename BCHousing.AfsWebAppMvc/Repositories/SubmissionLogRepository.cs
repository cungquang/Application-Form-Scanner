using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Models;
using BCHousing.AfsWebAppMvc.Servives.AfsDbContextService;
using Microsoft.EntityFrameworkCore;

namespace BCHousing.AfsWebAppMvc.Repositories
{
    public class SubmissionLogRepository : ISubmissionLogRepository
    {
        private readonly AfsDbContextService _dbContext;

        public SubmissionLogRepository(AfsDbContextService dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<SubmissionLog>> GetSubmissionLogs()
        {
            return await _dbContext.SubmissionLog.ToListAsync();
        }

        public async Task<int> CreateSubmissionLog(SubmissionLog newSubmissionLog) {
            
            _dbContext.SubmissionLog.Add(newSubmissionLog);
            return await _dbContext.SaveChangesAsync();
        }
    }

    public interface ISubmissionLogRepository
    {
        Task<IList<SubmissionLog>> GetSubmissionLogs();

        Task<int> CreateSubmissionLog(SubmissionLog newSubmissionLog);


    }
}

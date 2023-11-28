using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Models;
using BCHousing.AfsWebAppMvc.Servives.AfsDbContextService;
using Microsoft.EntityFrameworkCore;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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

        public async Task<SubmissionLog> GetSubmissionLog(string fileUrl)
        {
            var submissionLog = await _dbContext.SubmissionLog.FirstOrDefaultAsync(log => log.path_to_document == fileUrl);

            if (submissionLog == null)
            {
                throw new Exception($"no submissionLog with file Url {fileUrl} was found");
            }

            return submissionLog;

        }

        public async Task<int> CreateSubmissionLog(SubmissionLog newSubmissionLog) {
            
            _dbContext.SubmissionLog.Add(newSubmissionLog);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePathToFile(UpdateFilePath updateFilePath) {
            var LogToUpdate = _dbContext.SubmissionLog.FirstOrDefault(log => log.path_to_document == updateFilePath.CurrentFilePath);

            if (LogToUpdate != null) { 
                LogToUpdate.path_to_document = updateFilePath.NewFilePath;
            }

            return await _dbContext.SaveChangesAsync();

        }

    }

    public interface ISubmissionLogRepository
    {
        Task<IList<SubmissionLog>> GetSubmissionLogs();

        Task<int> CreateSubmissionLog(SubmissionLog newSubmissionLog);

        Task<SubmissionLog> GetSubmissionLog(string fileUrl);

        Task<int> UpdatePathToFile(UpdateFilePath updateFilePath);
    }
}

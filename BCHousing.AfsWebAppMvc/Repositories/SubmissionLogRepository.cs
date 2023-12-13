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

        public async Task<IList<SubmissionLog>> GetSubmissionLogsAsync()
        {
            return await _dbContext.SubmissionLog.ToListAsync();
        }

        public async Task<SubmissionLog> GetSubmissionLogAsync(string fileUrl)
        {
            var submissionLog = await _dbContext.SubmissionLog.FirstOrDefaultAsync(log => log.path_to_document == fileUrl);

            if (submissionLog == null)
            {
                throw new Exception($"no submissionLog with file Url {fileUrl} was found");
            }

            return submissionLog;

        }

        public async Task<int> CreateSubmissionLogAsync(SubmissionLog newSubmissionLog) {
            
            _dbContext.SubmissionLog.Add(newSubmissionLog);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdatePathToFileAsync(UpdateFilePath updateFilePath) {
            var logToUpdate = _dbContext.SubmissionLog.FirstOrDefault(log => log.path_to_document == updateFilePath.CurrentFilePath);

            if (logToUpdate != null) { 
                logToUpdate.path_to_document = updateFilePath.NewFilePath;
            }

            return await _dbContext.SaveChangesAsync();

        }

        public async Task<int> UpdateLogAfterExtractOCRAsync(UpdateLogAfterOCRExtraction updateLog) { 
            var logToUpdate = _dbContext.SubmissionLog.FirstOrDefault(log => log.path_to_document == updateLog.FileUrl);
            if (logToUpdate != null)
            {
                logToUpdate.avg_confidence_level = updateLog.AvgConfidenceScore;
                logToUpdate.is_read = updateLog.isRead;
                logToUpdate.path_to_analysis_report = updateLog.PathToAnalysisReport;
            }

            return await _dbContext.SaveChangesAsync();
        }

    }

    public interface ISubmissionLogRepository
    {
        Task<IList<SubmissionLog>> GetSubmissionLogsAsync();

        Task<int> CreateSubmissionLogAsync(SubmissionLog newSubmissionLog);

        Task<SubmissionLog> GetSubmissionLogAsync(string fileUrl);

        Task<int> UpdatePathToFileAsync(UpdateFilePath updateFilePath);

        Task<int> UpdateLogAfterExtractOCRAsync(UpdateLogAfterOCRExtraction updateLog);
    }
}

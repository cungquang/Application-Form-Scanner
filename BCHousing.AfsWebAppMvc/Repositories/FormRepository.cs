using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Servives.AfsDbContextService;
using Microsoft.EntityFrameworkCore;

namespace BCHousing.AfsWebAppMvc.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly AfsDbContextService _dbContext;

        public FormRepository(AfsDbContextService dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateFormRecord(Form newFormRecord)
        {
            _dbContext.Form.Add(newFormRecord);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IList<Form>> GetFormRecordAsync(Guid targetSubmissionId)
        {
            return await _dbContext.Form.Where(form => form.submissionId == targetSubmissionId).ToListAsync();
        }
    }

    public interface IFormRepository
    {
        Task<int> CreateFormRecord(Form newFormRecord);

        Task<IList<Form>> GetFormRecordAsync(Guid submissionId);
    }
}

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

        public async Task<int> CreateFormRecordAsync(Form newFormRecord)
        {
            _dbContext.Form.Add(newFormRecord);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IList<Form>> GetFormRecordAsync(Guid targetSubmissionId)
        {
            return await _dbContext.Form.Where(form => form.submissionId == targetSubmissionId).ToListAsync();
        }

        public async Task<int> UpdateFormBySubmissionIdAndSequenceAsync(Guid submissionId, int sequence, string? newValue)
        {
            var submission = _dbContext.Form.FirstOrDefault(form => form.submissionId == submissionId && form.sequence == sequence) ?? throw new Exception("Invalid input, the record does not exist in this context");
            submission.field_value = newValue;
            _dbContext.Entry(submission).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync();
        }
    }

    public interface IFormRepository
    {
        Task<int> CreateFormRecordAsync(Form newFormRecord);

        Task<IList<Form>> GetFormRecordAsync(Guid submissionId);

        Task<int> UpdateFormBySubmissionIdAndSequenceAsync(Guid submissionId, int sequence, string? newValue);
    }
}

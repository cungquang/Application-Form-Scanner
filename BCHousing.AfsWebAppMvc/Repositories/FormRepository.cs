using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Servives.AfsDbContextService;

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
    }

    public interface IFormRepository
    {
        Task<int> CreateFormRecord(Form newFormRecord);
    }
}

using Microsoft.EntityFrameworkCore;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDbContextService
{
    public class AfsDbContextService : DbContext
    {
        public AfsDbContextService(DbContextOptions<AfsDbContextService> options) : base(options) { }
    }
}

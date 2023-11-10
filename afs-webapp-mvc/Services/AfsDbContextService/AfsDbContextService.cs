using Microsoft.EntityFrameworkCore;

namespace afs_webapp_mvc.Services.AfsDbContextService
{
    public class AfsDbContextService : DbContext
    {
        public AfsDbContextService(DbContextOptions<AfsDbContextService> options) :base(options) { }
    }
}

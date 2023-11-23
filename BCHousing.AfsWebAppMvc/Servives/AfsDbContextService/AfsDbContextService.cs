using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BCHousing.AfsWebAppMvc.Servives.AfsDbContextService
{
    public class AfsDbContextService : DbContext
    {
        public AfsDbContextService(DbContextOptions<AfsDbContextService> options) : base(options)
        {

        }

        public virtual DbSet<SubmissionLog> SubmissionLog { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BCHousing.AfsWebAppMvc.Entities
{
    public class SubmissionLog : IEquatable<SubmissionLog>
    {
        [Key]
        public Guid submissionId { get; set; }
        public DateTime? timestamp { get; set; }
        public string? submit_by { get; set; }
        public string? document_name { get; set; }
        public long? document_size { get; set; }
        public string? user_declared_type { get; set; }
        public string? classify_type { get; set; }
        public bool? is_read { get; set; }
        public decimal? avg_confidence_level { get; set; }
        public string? path_to_document { get; set; }
        public string? path_to_analysis_report { get; set; }

        public bool Equals(SubmissionLog? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return submissionId == other.submissionId;
        }
    }
}

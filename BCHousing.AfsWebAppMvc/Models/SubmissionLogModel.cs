using System.Numerics;

namespace BCHousing.AfsWebAppMvc.Models
{
    public class SubmissionLogModel
    {
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
    }
}

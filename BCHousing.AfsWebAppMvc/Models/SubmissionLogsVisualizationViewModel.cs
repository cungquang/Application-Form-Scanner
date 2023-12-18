using BCHousing.AfsWebAppMvc.Entities;

namespace BCHousing.AfsWebAppMvc.Models
{
    public class SubmissionLogsVisualizationViewModel
    {
        public SubmissionLogsVisualizationViewModel() { }

        public SubmissionLogsVisualizationViewModel(IList<SubmissionLog>? submissionLogs) : base() {
            SubmissionLogs = submissionLogs;
            if (submissionLogs != null) NumberOfFile = submissionLogs.Count;
        }

        public long NumberOfFile {  get; set; }

        public IList<SubmissionLog>? SubmissionLogs { get; set; }

    }
}

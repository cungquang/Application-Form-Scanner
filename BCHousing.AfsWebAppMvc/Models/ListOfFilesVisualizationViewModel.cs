using BCHousing.AfsWebAppMvc.Entities;

namespace BCHousing.AfsWebAppMvc.Models
{
    public class ListOfFilesVisualizationViewModel
    {
        public long NumberOfFile {  get; set; }

        public IList<SubmissionLog>? SubmissionLogs { get; set; }

        public ListOfFilesVisualizationViewModel() { }

        public ListOfFilesVisualizationViewModel(IList<SubmissionLog>? submissionLogs) : base() {
            SubmissionLogs = submissionLogs;
            if (submissionLogs != null)
            {
                NumberOfFile = submissionLogs.Count;
            }
        }
    }
}

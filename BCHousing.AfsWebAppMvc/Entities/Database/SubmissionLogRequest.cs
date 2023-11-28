namespace BCHousing.AfsWebAppMvc.Entities.Database
{
    public class SubmissionLogRequest
    {
        public string? ClassifyType { get; set; }
        public string? FileUrl { get; set; }
    }

    public class UpdateFilePath { 
        public string? CurrentFilePath { get; set; }
        public string? NewFilePath { get; set;}
    }
}

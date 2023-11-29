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

    public class UpdateLogAfterOCRExtraction { 
        public string? FileUrl { get; set; }
        public bool? isRead {  get; set; }
        public decimal? AvgConfidenceScore { get; set; }
        public string? PathToAnalysisReport { get; set; }
    }
}

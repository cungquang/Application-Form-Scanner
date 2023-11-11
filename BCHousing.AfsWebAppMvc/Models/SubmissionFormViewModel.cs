namespace BCHousing.AfsWebAppMvc.Models
{
    public class SubmissionFormViewModel
    {
        private string? SubmissionID { get; set; }
        private string? DocumentName { get; set; }
        private long DocumentSize { get; set; }
        private long DocumentType { get; set; }
        private string? PathToDocument { get; set; }
        private bool IsRead { get; set; }
        private string? UserInputInJSON { get; set; }
    }
}

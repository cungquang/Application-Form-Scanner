using System.ComponentModel.DataAnnotations;

namespace BCHousing.AfsWebAppMvc.Entities.Session
{
    public class SubmissionViewInputModel
    {
        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? DocumentType { get; set; }

        public string? UploadFileName { get; set; }

        public long UploadFileSize { get; set; }
    }
}

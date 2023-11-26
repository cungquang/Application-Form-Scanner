using System.ComponentModel.DataAnnotations;

namespace BCHousing.AfsWebAppMvc.Entities.Session
{
    public class SubmissionViewInputModel
    {
        public SubmissionViewInputModel()
        {
            LastName = new();
            FirstName = new();
            DocumentType = new();
            UploadFileName = new();
            UploadFileSize = new();
            IsSuccess = new();
        }

        public List<string>? LastName { get; set; }

        public List<string>? FirstName { get; set; }

        public List<string>? DocumentType { get; set; }

        public List<string>? UploadFileName { get; set; }

        public List<long>? UploadFileSize { get; set; }

        public List<bool>? IsSuccess { get; set; }
    }
}

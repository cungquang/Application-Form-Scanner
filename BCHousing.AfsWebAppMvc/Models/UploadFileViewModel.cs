namespace BCHousing.AfsWebAppMvc.Models
{
    public class UploadFileViewModel
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? DocumentType { get; set; }
        public IFormFile UploadFile { get; set; }   
    }
}

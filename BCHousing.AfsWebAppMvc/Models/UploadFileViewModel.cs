using System.ComponentModel.DataAnnotations;

namespace BCHousing.AfsWebAppMvc.Models
{
    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "Please provide your last name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please provide your first name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please select a valid option")]
        [RegularExpression("^(Other|Safer|Rap)$", ErrorMessage = "Invalid selection type")]
        public string? DocumentType { get; set; }

        [Required(ErrorMessage = "Please select a file to submit")]
        public IFormFile UploadFile { get; set; }   
    }
}

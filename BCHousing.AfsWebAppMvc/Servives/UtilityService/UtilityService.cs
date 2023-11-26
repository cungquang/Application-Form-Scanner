using BCHousing.AfsWebAppMvc.Models;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace BCHousing.AfsWebAppMvc.Servives.UtilityService
{
    public static class UtilityService
    {
        public static Guid GenerateSystemGuid()
        {
            return Guid.NewGuid();
        }

        public static Guid GenerateBCHousingGuid(UploadFileViewModel model)
        {
            return Guid.NewGuid();
        }

        public static async Task<string> SerializeMetadataAsync(UploadFileViewModel model)
        {
            Dictionary<string, string> metadata = new()
            {
                ["SubmitBy"] = $"{model.FirstName} {model.LastName}",
                ["UserDeclaredType"] = $"{model.DocumentType}",
                ["DocumentName"] = $"{model.UploadFile.FileName}"
            };

            // Retun metadata in form of JSon string
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync<Dictionary<string, string>>(stream, metadata);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}

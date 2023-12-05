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

        public static Guid ConvertStringToGuid(string idValue)
        {
            if(!Guid.TryParse(idValue, out Guid validGuid))
            {
                throw new ArgumentException("Invalid input argument, cannot convert to Guid type");
            }
            return validGuid;
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

        public static async Task<Dictionary<string, string>> GetContainerAndBlobName(string blobUrl)
        {
            //Sample URL: https://afspocstorage.blob.core.windows.net/file-container/Other/2d5fe2e9-152a-46ae-befc-25cc936fcb1b.pdf
            //Sample URL: https://afspocstorage.blob.core.windows.net/staging-container/2d5fe2e9-152a-46ae-befc-25cc936fcb1b.pdf
            await Task.Delay(0);
            Dictionary<string, string> UrlParts = new();
            string[] parts = blobUrl.Split("/");
            UrlParts["Protocols"] = parts[0];
            UrlParts["Domain Name"] = parts[2];
            UrlParts["Container Name"] = parts[3];
            UrlParts["Folder Name"] = parts.Length > 5 ? parts[4] : "";
            UrlParts["Blob Name"] = parts[^1];

            return UrlParts;
        }
    }
}

namespace afs_webapp_mvc.Services.GenerateTemplateService
{
    public class GeneratePDFTemplate : IGenerateTemplateService
    {
        public async Task<string> GenerateTemplateAsync(string templateContent)
        {
            await Task.Delay(1000);
            return "";
        }
    }
}

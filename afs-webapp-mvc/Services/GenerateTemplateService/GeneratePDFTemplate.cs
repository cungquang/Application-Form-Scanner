namespace afs_webapp_mvc.Services.GenerateTemplateService
{
    public class GeneratePDFTemplate : IGenerateTemplateService<GeneratePDFTemplate>
    {
        public async Task<string> GenerateTemplateAsync(string templateContent)
        {
            Console.WriteLine("Print PDF document");
            await Task.Delay(1000);
            return "";
        }
    }
}

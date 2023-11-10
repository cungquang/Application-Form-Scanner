namespace afs_webapp_mvc.Services.GenerateTemplateService
{
    public class GenerateHTMLTemplate : IGenerateTemplateService<GenerateHTMLTemplate>
    {
        public async Task<string> GenerateTemplateAsync(string templateContent)
        {
            Console.WriteLine("Print HTML document");
            await Task.Delay(1000);
            return "";
        }
    }
}

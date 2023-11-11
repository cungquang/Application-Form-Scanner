namespace BCHousing.AfsWebAppMvc.Servives.GenerateTemplateService
{
    public class GenerateHTMLTemplate : IGenerateTemplateService<GenerateHTMLTemplate>
    {
        public async Task<string> GenerateTemplateAsync(string templateContent)
        {
            await Task.Delay(1000);
            return "Generate HTML template";
        }
    }
}

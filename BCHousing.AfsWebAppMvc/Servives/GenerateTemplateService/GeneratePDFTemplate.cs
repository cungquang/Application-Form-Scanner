namespace BCHousing.AfsWebAppMvc.Servives.GenerateTemplateService
{
    public class GeneratePDFTemplate : IGenerateTemplateService<GeneratePDFTemplate>
    {
        public async Task<string> GenerateTemplateAsync(string templateContent)
        {
            await Task.Delay(1000);
            return "Generate PDF Template";
        }
    }
}

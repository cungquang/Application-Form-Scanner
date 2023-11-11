namespace BCHousing.AfsWebAppMvc.Servives.GenerateTemplateService
{
    public interface IGenerateTemplateService<T> where T : class
    {
        /*
         * GenerateTemplateAsync
         * Description: 
         *  -> Generate a template per request (Ex: web view, pdf, excel...), and return to user with input data
         * Input:
         *  -> string templateContent: content will be loaded into template
         * Output:
         *  -> return a string - template with content
         */
        Task<string> GenerateTemplateAsync(string templateContent);
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace BCHousing.AfsOcrServerHandler
{
    public static class AfsOcrServerHandler
    {
        [FunctionName("AfsOcrServerHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            // Information for connecting AI Service
            string endpoint = "https://application-scanner-poc-di-service.cognitiveservices.azure.com/";
            string key = "ab5cf6c8b3264c698ef833de6d4f6d78";
            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            // Share Access Signature for Accessing the files in the blob storage
            // This will expire on 2023/12/22
            string shareAccessSignature = "?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2023-12-22T08:18:00Z&st=2023-11-18T00:18:00Z&spr=https&sig=Z%2BqyZZ%2By7xfGdZqYrjAmaTizNIKl4QpS%2FYZUevtO0XU%3D";


            // Step 1: Get the target file url for the event grid trigger
            string file_path = await GetUploadFilePath(req, log);
            string ocrService = IdentifiedOCRService(file_path);


            if (ocrService.Equals("Classification"))
            {
                // Step 2: Call AI Classification service 
                Uri fileUri = new Uri($"{file_path}{shareAccessSignature}");
                ClassifyDocumentOperation operation = await client.ClassifyDocumentFromUriAsync(WaitUntil.Completed, "SAFER_and_RAP_model", fileUri);
                AnalyzeResult classifiedResult = await OCRClassifyFile(client, fileUri, log);

                string fileType = ClassifyFileType(classifiedResult);
                log.LogInformation("Classified doc type is:" + fileType);

                // TODO: Store the classification result if something goes wrong

                // TODO: Step 3: Call moving file and updated SQL database API services
                // Move the classified file from staging folder to file folder
                // Call the Update the Database API
            }
            else {
                // TODO: Do the extraction service
                // TODO: Step 4: Call AI Exstraction service
                // TODO: Store the result if something goes wrong

                // TODO: Step 5: Call store the data to SQL DB API service 
            }


            return new OkObjectResult("Ok");
        }

        public static async Task<string> GetUploadFilePath(HttpRequest req, ILogger log)
        {
            string filePath = "";
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation("C# HTTP trigger function processed a request." +
                "Response:" + requestBody);

            // Deserialize the JSON string
            JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);

            // Access the value associated with the "url" key
            filePath = data["url"]?.ToString();
            log.LogInformation($"File path: {filePath}");

            return filePath;
        }

        public static string IdentifiedOCRService(string url) {
            // TODO: Identify which OCR service is needed
            // If the file path is from satgging container --> go to classification
            // e.g. https://afspocstorage.blob.core.windows.net/staging-container/RAP_training_data_03.pdf
            // If the file path is from file container --> go to extraction
            // e.g. https://afspocstorage.blob.core.windows.net/file-container/SAFER-RAP/RAP_training_data_04.pdf
            string ocrService = "Classification";
            

            return ocrService;
        }

        public static async Task<AnalyzeResult> OCRClassifyFile(DocumentAnalysisClient client, Uri fileUri, ILogger log) {
            ClassifyDocumentOperation operation = await client.ClassifyDocumentFromUriAsync(WaitUntil.Completed, "SAFER_and_RAP_model", fileUri);
            AnalyzeResult result = operation.Value;

            // Getting information from the Custom Classification result
            for (int i = 0; i < result.Documents.Count; i++)
            {
                log.LogInformation($"Document {i} : {result.Documents[i].DocumentType} : {result.Documents[i].Confidence}");
            }
            return result;
        }

        public static string ClassifyFileType(AnalyzeResult classifiedResult) {
            string fileType = "Other";
            if (classifiedResult.Documents.Count < 2)
            {
                double confidentScore = classifiedResult.Documents[0].Confidence;
                string docType = classifiedResult.Documents[0].DocumentType;
                // if the confidence score is lower than 90, it is either SAFER or RAP
                if (confidentScore < 0.9)
                {
                    fileType = "Other";
                }
                else {
                    fileType = docType;
                }
            }
            else {
               fileType = "Other";
            }
            return fileType;
        }
    }
}

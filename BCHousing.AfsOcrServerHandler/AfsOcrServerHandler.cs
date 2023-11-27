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
using System.Net.Http;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Text.Json;

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

            // Information for the REST API calls
            // TODO: Need Deploy Url
            string baseUrl = "https://bchousingafswebappmvc.azurewebsites.net/";

            // Share Access Signature for Accessing the files in the blob storage
            // This will expire on 2023/12/22
            string shareAccessSignature = "?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2023-12-22T08:18:00Z&st=2023-11-18T00:18:00Z&spr=https&sig=Z%2BqyZZ%2By7xfGdZqYrjAmaTizNIKl4QpS%2FYZUevtO0XU%3D";


            // Step 1: Get the target file url for the event grid trigger
            string file_path = await GetUploadFilePath(req, log);
            string ocrService = HandleOCRService(file_path, log);

            log.LogInformation($"OcrService: {ocrService}");

            if (ocrService.Equals("Classification"))
            {
                // Step 2: Call AI Classification service 
                Uri fileUri = new Uri($"{file_path}{shareAccessSignature}");
                AnalyzeResult classifiedResult = await OCRClassifyFile(client, fileUri, log);

                string fileType = ClassifyFileType(classifiedResult);
                log.LogInformation("Classified doc type is:" + fileType);

                // Store the classification result if something goes wrong
                log.LogInformation("Storing classification result...");
                await StoreOCRClassificationResult(fileType, file_path, baseUrl, log);

                // Step 3: Call moving file and updated SQL database API services
                // Move the classified file from staging folder to file folder
                log.LogInformation("Moving file...");
                await MoveStagingBlob(file_path, baseUrl, fileType, log);
                

            }
            else if(ocrService.Equals("Extraction"))
            {
                log.LogInformation("Stop testing extraction for now...");
                /*// TODO: Step 4: Call AI Exstraction service
                // Get the extraction model first
                string extractionModel = "SAFER_Model_v3";

                // (Test after deploy web app) TODO: Get the classify type for the file
                *//*
                string model = await IdentifyExtractionModel(file_path, baseUrl, log);
                *//*
                string model = "SAFER";

                if (!model.Equals("None"))
                {
                    if (model.Equals("SAFER"))
                    {
                        extractionModel = "SAFER_Model_v3";
                    }
                    else
                    {
                        extractionModel = "RAP_Model_v3";
                    }

                    Uri fileUri = new Uri($"{file_path}{shareAccessSignature}");
                    AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, extractionModel, fileUri);
                    AnalyzeResult result = operation.Value;
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                    log.LogInformation(jsonString);
                    // CheckExtractionResult(result, log);


                }
                else 
                {
                    log.LogInformation("No model found");                
                }

                //  (Test after deploy web app) TODO: Step 5: Call store the data to SQL DB API service */
            }
            else
            {
                // do nothing if triggered by Other folder
                log.LogInformation("Triggered from Other folder in file-container");
            }


            return new OkObjectResult("Ok");
        }

        public static async Task<string> GetUploadFilePath(HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation("C# HTTP trigger function processed a request." +
                "Response:" + requestBody);

            // Deserialize the JSON string
            JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);

            // Access the value associated with the "url" key
            var filePath = data["url"]?.ToString();
            log.LogInformation($"File path: {filePath}");

            return filePath;
        }

        public static string HandleOCRService(string url, ILogger log) {
            // Identify which OCR service is needed
            // If the file path is from satgging container --> go to classification
            // e.g. https://afspocstorage.blob.core.windows.net/staging-container/RAP_training_data_03.pdf --> length 5
            // If the file path is from file container --> go to extraction
            // e.g. https://afspocstorage.blob.core.windows.net/file-container/SAFER-RAP/RAP_training_data_04.pdf --> length 6
            string ocrService = "Other/Nothing"; // do nothing when trigger from Other container

            string[] urlInformation = url.Split('/');

            for (int i = 0; i < urlInformation.Length; i++) {
                if (urlInformation[i].Contains("container")) {
                    if (urlInformation[i].Equals("staging-container"))
                    {
                        ocrService = "Classification";
                    }
                    else if (urlInformation[i].Equals("file-container"))
                    {
                        if (urlInformation[i + 1].Equals("SAFER-RAP"))
                        {
                            ocrService = "Extraction";
                        }
                    }
                    else {
                        log.LogInformation($"Did not get the container path: {urlInformation[i]}");
                    }
                    break;
                }
            }

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

        public static void CheckExtractionResult(AnalyzeResult result, ILogger log) {

            log.LogInformation($"Document was analyzed with model with ID: {result.ModelId}");
            foreach (AnalyzedDocument document in result.Documents)
            {
                log.LogInformation($"Document of type: {document.DocumentType}");

                foreach (KeyValuePair<string, DocumentField> fieldKvp in document.Fields)
                {
                    string fieldName = fieldKvp.Key;
                    DocumentField field = fieldKvp.Value;

                    log.LogInformation($"Field '{fieldName}': ");

                    log.LogInformation($"  Content: '{field.Content}'");
                    log.LogInformation($"  Confidence: '{field.Confidence}'");
                }
            }

            // Iterate over lines and selection marks on each page
            foreach (DocumentPage page in result.Pages)
            {
                log.LogInformation($"Lines found on page {page.PageNumber}");
                foreach (var line in page.Lines)
                {
                    log.LogInformation($"  {line.Content}");
                }

                log.LogInformation($"Selection marks found on page {page.PageNumber}");
                foreach (var selectionMark in page.SelectionMarks)
                {
                    log.LogInformation($"  Selection mark is '{selectionMark.State}' with confidence {selectionMark.Confidence}");
                }
            }

            // Iterate over the document tables
            for (int i = 0; i < result.Tables.Count; i++)
            {
                log.LogInformation($"Table {i + 1}");
                foreach (var cell in result.Tables[i].Cells)
                {
                    log.LogInformation($"  Cell[{cell.RowIndex}][{cell.ColumnIndex}] has content '{cell.Content}' with kind '{cell.Kind}'");
                }
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////        API Calls           ////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static async Task StoreOCRClassificationResult(string classifyType, string fileUrl, string baseUrl, ILogger log) {
            // api/Database/CreateSubmissionLog
            try
            {
                string endpoint = "api/Database/CreateSubmissionLog";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();

                var requestData = new
                {
                    ClassifyType = classifyType,
                    FileUrl = fileUrl
                };

                // Serialize the request data to JSON
                string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true });
                HttpContent requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(requestUri, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    log.LogInformation("New Record inserted.");
                    log.LogInformation(responseBody);
                }
                else
                {
                    // If the request fails, display the status code
                    log.LogInformation($"Failed with status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException e)
            {
                log.LogInformation($"Request error: {e.Message}");
            }
           
        }

        public static async Task MoveStagingBlob(string fileUrl, string baseUrl, string fileType, ILogger log) {
            // api/BlobStorage/MoveStagingBlob/<filename>
            string[] fileInformation = fileUrl.Split('/');
            string filename = fileInformation[fileInformation.Length - 1];
            string destinationFolder = "SAFER-RAP";

            if (fileType.Equals("Other")) {
                destinationFolder = "Other";
            }

            try
            {
                string endpoint = $"api/BlobStorage/MoveStagingBlob/{filename}";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();

                var requestData = new
                {
                    DestinationContainer = "file-container",
                    DestinationFolder = destinationFolder
                };

                // Serialize the request data to JSON
                string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true });
                HttpContent requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(requestUri, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    log.LogInformation("Staging File have moved.");
                    log.LogInformation(responseBody);
                }
                else
                {
                    // If the request fails, display the status code
                    log.LogInformation($"Failed with status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException e)
            {
                log.LogInformation($"Request error: {e.Message}");
            }
        }

        public static async Task<string> IdentifyExtractionModel(string fileUrl, string baseUrl, ILogger log) {
            // api/Database/GetSubmissionLogByUrl
            string model = "None";
            try 
            {
                string endpoint = $"api/Database/GetSubmissionLogByUrl?fileUrl={fileUrl}";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

                    // Extract the "name" field from the response
                    string classify_type = jsonResponse.classify_type;
                    model = classify_type.Trim();
                }
                else
                {
                    // If the request fails, display the status code
                    log.LogInformation($"Failed with status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException e)
            {
                log.LogInformation($"Request error: {e.Message}");
            }

            return model;
        }
    
    }
}

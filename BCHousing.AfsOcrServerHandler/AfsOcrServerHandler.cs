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
using System.Collections.Generic;
using System.Text.Json;
using BCHousing.AfsOcrServerHandler.Model;

namespace BCHousing.AfsOcrServerHandler
{
    public static class AfsOcrServerHandler
    {
        [FunctionName("AfsOcrServerHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Information for the REST API calls
            string baseUrl = "https://bchousingafswebappmvc.azurewebsites.net/";

            // Information for connecting AI Service
            string endpoint = "https://application-scanner-poc-di-service.cognitiveservices.azure.com/";
            string key = "ab5cf6c8b3264c698ef833de6d4f6d78";
            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            // Share Access Signature for Accessing the files in the blob storage
            // This will expire on 2023/12/22
            string shareAccessSignature = "?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2023-12-22T08:18:00Z&st=2023-11-18T00:18:00Z&spr=https&sig=Z%2BqyZZ%2By7xfGdZqYrjAmaTizNIKl4QpS%2FYZUevtO0XU%3D";


            // Step 1: Get the target file url for the event grid trigger
            // Only process file from staging file
            string filePath = await GetUploadFilePathAsync(req, log);

            bool runOcrService = ShouldRunOCRService(filePath, log);
            string ocrService = "";


            if (runOcrService) {
                ocrService = HandleOCRService(filePath, log);
                log.LogInformation($"OcrService: {ocrService}");

                // Step 2: Run Classification serive 
                // Get user declare type
                // Check user declare type
                string userDeclareType = await GetUserDeclareTypeFromBlobAsync(baseUrl, filePath, log);
                log.LogInformation($"User Declare Type: {userDeclareType}");
                string newFilePath;
                if (userDeclareType.Equals("Safer") || userDeclareType.Equals("Rap"))
                {
                    // Run classificaiton
                    // Step 3: Call AI Classification service 
                    Uri fileUri = new Uri($"{filePath}{shareAccessSignature}");
                    AnalyzeResult classifiedResult = await OCRClassifyFileAsync(client, fileUri, log);

                    string fileType = ClassifyFileType(classifiedResult, log);
                    log.LogInformation("Classified doc type is:" + fileType);

                    // Store the classification result if something goes wrong
                    log.LogInformation("Storing classification result...");
                    await StoreOCRClassificationResultAsync(fileType, filePath, baseUrl, log);

                    // Step 4: Call moving file and updated SQL database API services
                    // Move the classified file from staging folder to file folder
                    log.LogInformation("Moving file...");
                    newFilePath = await MoveStagingBlobAsync(filePath, baseUrl, fileType, log);
                }
                else
                {
                    // User declare the file is "Other"
                    // Create a file with no classification result
                    await StoreOCRClassificationResultAsync("Unclassified", filePath, baseUrl, log);
                    // Move the file to Other folder directly
                    // Move the classified file from staging folder to file folder
                    log.LogInformation("Moving file...");
                    newFilePath = await MoveStagingBlobAsync(filePath, baseUrl, "Other", log);

                }

                // Step 5: Update file_to_path in the database
                log.LogInformation($"New File Path: {newFilePath}");
                await UpdateFilePathAsync(baseUrl, filePath, newFilePath, log);

                // update the staging-container file path to file-container file path
                filePath = newFilePath;

                // Step 6: Run OCR extraction service
                ocrService = HandleOCRService(filePath, log);
                log.LogInformation($"OcrService: {ocrService}");

                if (ocrService.Equals("Extraction"))
                {
                    // Step 7: Call AI Exstraction service
                    // Get the extraction model first
                    string extractionModel = "SAFER_Model_v4_template";

                    string model = await IdentifyExtractionModelAsync(filePath, baseUrl, log);
                    log.LogInformation($"Model name: {model}");


                    if (!model.Equals("None"))
                    {
                        if (model.Equals("SAFER"))
                        {
                            extractionModel = "SAFER_Model_v4_template";
                        }
                        else
                        {
                            extractionModel = "RAP_Model_v4_template";
                        }

                        // Step 8: Call store the data to SQL DB API service 
                        // Run actual OCR Extraction service
                        Uri fileUri = new Uri($"{filePath}{shareAccessSignature}");
                         AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, extractionModel, fileUri);
                         AnalyzeResult result = operation.Value;
                         List<ExtractDataModel> extractResult = GetExtractionResult(result, log);

                        // Run sample extraction data for other API test
                        /*List<ExtractDataModel> extractResult = new List<ExtractDataModel>() {
                            new ExtractDataModel { Key = "22", Content = "2013-10-19.", Confidence = 0.99f },
                            new ExtractDataModel { Key = "96", Content = "loveandpeace@gmail.com", Confidence = 0.99f },
                            new ExtractDataModel { Key = "57", Content = null, Confidence = 0.979f },
                            new ExtractDataModel { Key = "214", Content = ":selected:", Confidence = 0.994f }
                        };*/
                        string extractJsonResult = await StoreExtractionResultAsync(baseUrl, filePath, extractResult, log);

                        // Store result.json to blob
                        string pathToAnalysisReport = await StoreExtractJsonResultAsync(baseUrl, extractJsonResult, log);
                        log.LogInformation($"Path to analysis report: {pathToAnalysisReport}");

                        // Step 9: Update Database (is_read, avg_confidence_score and path_to_analysis_report)
                        decimal avgConfidenceScore = CalculateAvgConfidenceScore(extractResult);
                        log.LogInformation($"avgConfidenceScore: {avgConfidenceScore}");

                        bool isRead = true;
                        await UpdateLogAfterOCRExtractionAsync(baseUrl, filePath, isRead, avgConfidenceScore, pathToAnalysisReport, log);

                    }
                    else
                    {
                        // file move to Other folder
                        // no need to run any extraction service
                        log.LogInformation("No need to run OCR.");
                    }
                }

            }
        


            return new OkObjectResult("Ok");
        }

        public static async Task<string> GetUploadFilePathAsync(HttpRequest req, ILogger log)
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

        public static bool ShouldRunOCRService(string url, ILogger log) {
            // only process the file trigger from stagging-container
            // e.g. https://afspocstorage.blob.core.windows.net/staging-container/RAP_training_data_03.pdf

            string container = url.Split('/')[^2];
            if (container.Equals("staging-container")) {
                return true;
            }

            return false;
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

        public static async Task<AnalyzeResult> OCRClassifyFileAsync(DocumentAnalysisClient client, Uri fileUri, ILogger log) {
            ClassifyDocumentOperation operation = await client.ClassifyDocumentFromUriAsync(WaitUntil.Completed, "SAFER_and_RAP_model", fileUri);
            AnalyzeResult result = operation.Value;

            // Getting information from the Custom Classification result
            for (int i = 0; i < result.Documents.Count; i++)
            {
                log.LogInformation($"Document {i} : {result.Documents[i].DocumentType} : {result.Documents[i].Confidence}");
            }
            return result;
        }

        public static string ClassifyFileType(AnalyzeResult classifiedResult, ILogger log) {
            string fileType = "Other";
            if (classifiedResult.Documents.Count < 2)
            {
                double confidentScore = classifiedResult.Documents[0].Confidence;
                string docType = classifiedResult.Documents[0].DocumentType;
                // if the confidence score is lower than 90, it is either SAFER or RAP
                log.LogInformation($"docType: {docType}; confidentScore: {confidentScore}");
                if (confidentScore < 0.99)
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

        public static List<ExtractDataModel> GetExtractionResult(AnalyzeResult result, ILogger log) {

            log.LogInformation($"Document was analyzed with model with ID: {result.ModelId}");
            List<ExtractDataModel> formDatas = new List<ExtractDataModel>();
            foreach (AnalyzedDocument document in result.Documents)
            {
                Console.WriteLine($"Document of type: {document.DocumentType}");
                int count = 0;


                foreach (KeyValuePair<string, DocumentField> fieldKvp in document.Fields)
                {
                    string fieldName = fieldKvp.Key;
                    DocumentField field = fieldKvp.Value;
                    ExtractDataModel data = new ExtractDataModel() { Key = fieldName, Content = field.Content, Confidence = field.Confidence };
                    formDatas.Add(data);
                    count++;
                }

                log.LogInformation($"Total counts: {count}");
            }
            log.LogInformation($"formDatas data: {formDatas}");

            return formDatas;
        }

        public static decimal CalculateAvgConfidenceScore(List<ExtractDataModel> extractResult) { 
            float? totalConfidenceScore = 0;
            float count = 0;
            foreach (ExtractDataModel data in extractResult) {
                count++;
                totalConfidenceScore += data.Confidence;
            }

            float? ConfidenceScore = totalConfidenceScore / count;
            decimal avgConfidenceScore = (decimal)ConfidenceScore;

            return avgConfidenceScore;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////        API Calls           ////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static async Task StoreOCRClassificationResultAsync(string classifyType, string fileUrl, string baseUrl, ILogger log) {
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

        public static async Task<string> MoveStagingBlobAsync(string fileUrl, string baseUrl, string fileType, ILogger log) {
            // api/BlobStorage/MoveStagingBlob/<filename>
            string[] fileInformation = fileUrl.Split('/');
            string filename = fileInformation[fileInformation.Length - 1];
            string destinationFolder = "SAFER-RAP";
            string newFilePath ="No path"; 

            if (fileType.Equals("Other")) {
                destinationFolder = "Other";
            }

            try
            {
                string endpoint = $"api/BlobStorage/MoveStagingBlob";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();

                var requestData = new
                {
                    SourceURL = fileUrl,
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
                    //log.LogInformation($"responseBody: {responseBody}");
                    newFilePath = responseBody;
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

            return newFilePath;
        }

        public static async Task<string> IdentifyExtractionModelAsync(string fileUrl, string baseUrl, ILogger log) {
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
                    log.LogInformation($"IdentifyExtractionModel: Failed with status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException e)
            {
                log.LogInformation($"Request error: {e.Message}");
            }

            return model;
        }

        public static async Task<string> GetUserDeclareTypeFromBlobAsync(string baseUrl, string filePath, ILogger log) {
            // api/BlobStorage/Metadata
            string model = "Other";
            try
            {
                string endpoint = $"api/BlobStorage/Metadata?url={filePath}";
                string requestUri = baseUrl + endpoint;
                
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                    

                    // Extract the "name" field from the response
                    string userDeclaredType = jsonResponse.UserDeclaredType;
                    log.LogInformation($"userDeclaredType (from API): {userDeclaredType}");
                    model = userDeclaredType.Trim();
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

        public static async Task UpdateFilePathAsync(string baseUrl, string curFilePath, string newFilePath, ILogger log) {
            // api/Database/UpdatePathToFile
            try
            {
                string endpoint = "api/Database/UpdatePathToFile";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();

                var requestData = new
                {
                    CurrentFilePath = curFilePath,
                    NewFilePath = newFilePath
                };

                // Serialize the request data to JSON
                string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true });
                HttpContent requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(requestUri, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
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

        public static async Task<string> StoreExtractionResultAsync(string baseUrl, string filePath, List<ExtractDataModel> extarctData, ILogger log) {

            string jsonResult = "";
            try
            {
                string endpoint = "api/Database/CreateFormRecord";
                string apiUrl = baseUrl + endpoint;
                HttpClient httpClient = new HttpClient();

                // Serialize the request data to JSON
                // actual extraction result
                var requestData = new
                 {
                     fileUrl = filePath,
                     formDatas = extarctData
                 };

                string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true });

                // Testing extraction result
                /*string jsonStart = "{ \r\n  \"fileUrl\": \"";
                string jsonfileUrl = "\",";
                string jsonDatas = "\"formDatas\": [ { \"Key\": \"22\", \"Content\": \"2013-10-19.\", \"Confidence\": 0.99 }, { \"Key\": \"96\", \"Content\": \"loveand peace@gmail.com\", \"Confidence\": 0.99 }, { \"Key\": \"57\", \"Content\": null, \"Confidence\": 0.979 }, { \"Key\": \"214\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"190\", \"Content\": \"Michael St. James\", \"Confidence\": 0.948 }, { \"Key\": \"54\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"29\", \"Content\": \"19/10/2023\", \"Confidence\": 0.992 }, { \"Key\": \"154\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"14\", \"Content\": \"Yes\", \"Confidence\": 0.99 }, { \"Key\": \"180\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"97\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"58\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"59\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"35\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"49\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"34\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"87\", \"Content\": \":selected:\", \"Confidence\": 0.995 }, { \"Key\": \"63\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"45\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"43\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"13\", \"Content\": \"Female\", \"Confidence\": 0.992 }, { \"Key\": \"181\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"145\", \"Content\": \":unselected:\", \"Confidence\": 0.991 }, { \"Key\": \"130\", \"Content\": null, \"Confidence\": 0.993 }, { \"Key\": \"216\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"223\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"221\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"184\", \"Content\": \"2023/10/19\", \"Confidence\": 0.99 }, { \"Key\": \"80\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"121\", \"Content\": \":selected:\", \"Confidence\": 0.991 }, { \"Key\": \"46\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"162\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"113\", \"Content\": \"George Cooper\", \"Confidence\": 0.99 }, { \"Key\": \"51\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"172\", \"Content\": \"500\", \"Confidence\": 0.977 }, { \"Key\": \"169\", \"Content\": \"1500\", \"Confidence\": 0.994 }, { \"Key\": \"23\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"183\", \"Content\": \"Mans. James.\", \"Confidence\": 0.99 }, { \"Key\": \"188\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"72\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"88\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"126\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"218\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"209\", \"Content\": \"2023-10-19.\", \"Confidence\": 0.992 }, { \"Key\": \"151\", \"Content\": \":selected:\", \"Confidence\": 0.995 }, { \"Key\": \"100\", \"Content\": \"( ) -\", \"Confidence\": 0.99 }, { \"Key\": \"17\", \"Content\": \"Michael St. James\", \"Confidence\": 0.943 }, { \"Key\": \"55\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"147\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"42\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"19\", \"Content\": \"2023-10-19\", \"Confidence\": 0.99 }, { \"Key\": \"161\", \"Content\": \":selected:\", \"Confidence\": 0.995 }, { \"Key\": \"210\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"8\", \"Content\": \"515-666-2541\", \"Confidence\": 0.99 }, { \"Key\": \"163\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"7\", \"Content\": \"Yes\", \"Confidence\": 0.99 }, { \"Key\": \"171\", \"Content\": \"Self- employment\", \"Confidence\": 0.995 }, { \"Key\": \"44\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"98\", \"Content\": \"( ) -\", \"Confidence\": 0.99 }, { \"Key\": \"86\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"187\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"5\", \"Content\": \"63\", \"Confidence\": 0.99 }, { \"Key\": \"194\", \"Content\": \"Michael St. James\", \"Confidence\": 0.947 }, { \"Key\": \"205\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"79\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"179\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"83\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"68\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"82\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"207\", \"Content\": \"George Camper\", \"Confidence\": 0.953 }, { \"Key\": \"36\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"139\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"112\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"222\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"6\", \"Content\": \"Male\", \"Confidence\": 0.99 }, { \"Key\": \"142\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"189\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"208\", \"Content\": \"778-525-6491\", \"Confidence\": 0.947 }, { \"Key\": \"152\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"117\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"107\", \"Content\": \"V5X 3T4\", \"Confidence\": 0.99 }, { \"Key\": \"25\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"196\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"84\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"191\", \"Content\": \"875\", \"Confidence\": 0.993 }, { \"Key\": \"47\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"212\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"103\", \"Content\": \"53\", \"Confidence\": 0.99 }, { \"Key\": \"220\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"21\", \"Content\": \"Jurisa Sum\", \"Confidence\": 0.993 }, { \"Key\": \"4\", \"Content\": \"15/12/1960\", \"Confidence\": 0.991 }, { \"Key\": \"201\", \"Content\": \"2000\", \"Confidence\": 0.992 }, { \"Key\": \"101\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"168\", \"Content\": \"Full-time employment\", \"Confidence\": 0.99 }, { \"Key\": \"136\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"140\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"165\", \"Content\": \"Old Age Security, Guaranteed Income Supplement, and Allowance for the Survivor (if applicable)\", \"Confidence\": 0.99 }, { \"Key\": \"24\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"116\", \"Content\": \":selected:\", \"Confidence\": 0.991 }, { \"Key\": \"74\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"144\", \"Content\": \":unselected:\", \"Confidence\": 0.991 }, { \"Key\": \"200\", \"Content\": \"25/12/1963\", \"Confidence\": 0.992 }, { \"Key\": \"217\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"108\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"110\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"204\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"76\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"31\", \"Content\": \"778-555-6541\", \"Confidence\": 0.992 }, { \"Key\": \"120\", \"Content\": \"2000\", \"Confidence\": 0.994 }, { \"Key\": \"111\", \"Content\": null, \"Confidence\": 0.99 }, { \"Key\": \"67\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"114\", \"Content\": \"778-555-6499\", \"Confidence\": 0.99 }, { \"Key\": \"202\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"38\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"104\", \"Content\": \"6533\", \"Confidence\": 0.99 }, { \"Key\": \"186\", \"Content\": \"2021/10/19.\", \"Confidence\": 0.991 }, { \"Key\": \"16\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"146\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"33\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"99\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"105\", \"Content\": \"Fraser St.\", \"Confidence\": 0.99 }, { \"Key\": \"10\", \"Content\": \"Janice\", \"Confidence\": 0.99 }, { \"Key\": \"115\", \"Content\": \"6810 Main Street, Vancouver, BC, VEXOAI\", \"Confidence\": 0.99 }, { \"Key\": \"94\", \"Content\": \"(\", \"Confidence\": 0.99 }, { \"Key\": \"158\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"28\", \"Content\": \"25/12/1963\", \"Confidence\": 0.992 }, { \"Key\": \"119\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"15\", \"Content\": \":selected:\", \"Confidence\": 0.992 }, { \"Key\": \"177\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"64\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"93\", \"Content\": \"(778)- 585-4346\", \"Confidence\": 0.946 }, { \"Key\": \"131\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"128\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"155\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"195\", \"Content\": \"778-254-7821\", \"Confidence\": 0.99 }, { \"Key\": \"92\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"176\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"143\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"122\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"141\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"39\", \"Content\": null, \"Confidence\": 0.99 }, { \"Key\": \"52\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"11\", \"Content\": \"15/07/1960\", \"Confidence\": 0.99 }, { \"Key\": \"159\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"124\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"133\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"173\", \"Content\": \"200\", \"Confidence\": 0.968 }, { \"Key\": \"138\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"18\", \"Content\": \"Mm. S.J\", \"Confidence\": 0.99 }, { \"Key\": \"60\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"66\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"65\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"148\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"182\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"102\", \"Content\": \"( ) -\", \"Confidence\": 0.99 }, { \"Key\": \"9\", \"Content\": \"St. James\", \"Confidence\": 0.99 }, { \"Key\": \"213\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"75\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"109\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"219\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"2\", \"Content\": \"St. James\", \"Confidence\": 0.99 }, { \"Key\": \"56\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"135\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"89\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"85\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"175\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"78\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"91\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"197\", \"Content\": \"George Cooper\", \"Confidence\": 0.884 }, { \"Key\": \"26\", \"Content\": \"63 years\", \"Confidence\": 0.99 }, { \"Key\": \"81\", \"Content\": null, \"Confidence\": 0.979 }, { \"Key\": \"134\", \"Content\": \":selected:\", \"Confidence\": 0.993 }, { \"Key\": \"41\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"70\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"127\", \"Content\": \":selected:\", \"Confidence\": 0.992 }, { \"Key\": \"153\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"48\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"198\", \"Content\": \"53, 6533, Vancouver\", \"Confidence\": 0.953 }, { \"Key\": \"132\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"215\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"199\", \"Content\": \"Michael St. James\", \"Confidence\": 0.949 }, { \"Key\": \"1\", \"Content\": \"33-415-6571\", \"Confidence\": 0.99 }, { \"Key\": \"37\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"164\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"69\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"157\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"30\", \"Content\": \"Allan Sparks\", \"Confidence\": 0.99 }, { \"Key\": \"160\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"178\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"193\", \"Content\": \"4910 2511 3452 5421\", \"Confidence\": 0.948 }, { \"Key\": \"123\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"125\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"174\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"90\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"156\", \"Content\": \":selected:\", \"Confidence\": 0.995 }, { \"Key\": \"12\", \"Content\": \"63\", \"Confidence\": 0.99 }, { \"Key\": \"62\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"73\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"32\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"40\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"170\", \"Content\": \"2300\", \"Confidence\": 0.993 }, { \"Key\": \"203\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"50\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"150\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"95\", \"Content\": \"( ) -\", \"Confidence\": 0.949 }, { \"Key\": \"185\", \"Content\": \"\", \"Confidence\": 0.995 }, { \"Key\": \"20\", \"Content\": \"Janice St. James\", \"Confidence\": 0.99 }, { \"Key\": \"129\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"118\", \"Content\": \":unselected:\", \"Confidence\": 0.992 }, { \"Key\": \"224\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"61\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"206\", \"Content\": \":unselected:\", \"Confidence\": 0.994 }, { \"Key\": \"106\", \"Content\": \"Vancouver\", \"Confidence\": 0.99 }, { \"Key\": \"192\", \"Content\": \"6541\", \"Confidence\": 0.992 }, { \"Key\": \"27\", \"Content\": \"Current address\", \"Confidence\": 0.99 }, { \"Key\": \"149\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"137\", \"Content\": \":unselected:\", \"Confidence\": 0.991 }, { \"Key\": \"167\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"77\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"3\", \"Content\": \"Michael\", \"Confidence\": 0.99 }, { \"Key\": \"211\", \"Content\": \":unselected:\", \"Confidence\": 0.993 }, { \"Key\": \"71\", \"Content\": null, \"Confidence\": 0.937 }, { \"Key\": \"225\", \"Content\": \":selected:\", \"Confidence\": 0.994 }, { \"Key\": \"53\", \"Content\": \":selected:\", \"Confidence\": 0.995 }, { \"Key\": \"166\", \"Content\": null, \"Confidence\": 0.937 } ] \r\n }";
                string jsonBody = jsonStart + filePath + jsonfileUrl + jsonDatas;*/

                // Return Json result for storing purpose
                jsonResult = jsonBody;
                log.LogInformation($"Extraction data: {jsonBody}");

                HttpContent requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, requestBody);

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

            return jsonResult;
        }

        public static async Task<string> StoreExtractJsonResultAsync(string baseUrl, string jsonResult, ILogger log) {
            // api/BlobStorage/UploadBlobAsync
            string path_to_analysis_report = "";
            try
            {
                string endpoint = "api/BlobStorage/UploadBlobAsync";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();

                string blobname = "Analysis/" + Guid.NewGuid().ToString() + ".json";
                var requestData = new
                {
                    ContainerName = "file-container",
                    BlobName = blobname,
                    BlobContent = jsonResult,
                    Metadata = ""
                };

                // Serialize the request data to JSON
                string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true });
                HttpContent requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(requestUri, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    log.LogInformation(responseBody);
                    path_to_analysis_report = responseBody;
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

            return path_to_analysis_report;
        }

        public static async Task UpdateLogAfterOCRExtractionAsync(string baseUrl, string filePath, bool isRead, decimal avgConfildenceScore, string pathToAnalysisReport, ILogger log) {
            // api/BlobStorage/UpdateSubmissionLogAfterOCRExtraction
            try
            {
                string endpoint = "api/Database/UpdateSubmissionLogAfterOCRExtraction";
                string requestUri = baseUrl + endpoint;

                HttpClient client = new HttpClient();

                
                var requestData = new
                {
                    FileUrl = filePath,
                    isRead = isRead,
                    AvgConfidenceScore = avgConfildenceScore,
                    PathToAnalysisReport = pathToAnalysisReport
                };

                // Serialize the request data to JSON
                string jsonBody = System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true });
                HttpContent requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(requestUri, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
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
    }
}

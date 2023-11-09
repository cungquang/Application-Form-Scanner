// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using System.Net.Http;

namespace afs_trigger_staging_container
{
    public static class TriggerStagingContainer
    {
        private static readonly HttpClient HttpClients = new HttpClient();
        private static readonly string URL_Communicator = Environment.GetEnvironmentVariable("URL_Communicator");

        [FunctionName("TriggerStagingContainer")]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            string eventData = eventGridEvent.Data.ToString();

            var response = HttpClients.PostAsync(URL_Communicator, new StringContent(eventData)).Result;
            log.LogInformation($"HTTP POST: {response.StatusCode}");

            // print log - for tracking purposes
            log.LogInformation(eventData);
        }
    }
}

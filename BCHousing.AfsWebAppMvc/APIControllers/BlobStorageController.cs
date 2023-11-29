using BCHousing.AfsWebAppMvc.Controllers;
using BCHousing.AfsWebAppMvc.Entities.BlobStorage;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using BCHousing.AfsWebAppMvc.Servives.UtilityService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Text;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BCHousing.AfsWebAppMvc.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<HomeController> _logger;

        public BlobStorageController(ILogger<HomeController> logger, 
            IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        // GET: api/<BlobStorageAPIController>
        [HttpGet("Metadata")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Metadata([FromQuery] string url)
        {
            try
            {
                Dictionary<string, string> UrlParts = await UtilityService.GetContainerAndBlobName(url);
                string blobFullPath = string.IsNullOrEmpty(UrlParts["Folder Name"]) ? UrlParts["Blob Name"] : $"{UrlParts["Folder Name"]}/{UrlParts["Blob Name"]}";
                
                // Retrieve metadata from the blob
                string response = JsonSerializer.Serialize(await _blobStorageService.GetMetaDataAsync(UrlParts["Container Name"], blobFullPath));

                return StatusCode(StatusCodes.Status200OK, response);
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }

        // POST: api/<BlobStorageAPIController>
        [HttpPost("Metadata")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Metadata([FromBody] MetadataPostRequest requestBody)
        {
            try
            {
                Dictionary<string, string> UrlParts = await UtilityService.GetContainerAndBlobName(requestBody.URL);
                string blobFullPath = string.IsNullOrEmpty(UrlParts["Folder Name"]) ? UrlParts["Blob Name"] : $"{UrlParts["Folder Name"]}/{UrlParts["Blob Name"]}";

                // Update metadta
                string response = await _blobStorageService.WriteMetaDataAsync(UrlParts["Container Name"], blobFullPath, metadata: requestBody.Metadata) ?
                    "Successfully update metadata" : "Fail to update metadata";

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }

        // POST api/<BlobStorageAPIController>/StagingBlob}
        [HttpPost("MoveStagingBlob")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MoveStagingBlob([FromBody] CopyBlobPostRequest requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody.SourceURL) || string.IsNullOrEmpty(requestBody.DestinationContainer))
                {
                    throw new BadHttpRequestException("SourceURL/DestinationContainer cannot be null or empty");
                }
                
                string destinationFileName = "";
                string sourceFileName = requestBody.SourceURL.Split("/")[^1];

                if (!string.IsNullOrEmpty(requestBody.DestinationFolder)) destinationFileName = $"{requestBody.DestinationFolder}/{sourceFileName}";

                //Call Blobstorage service
                string response = await _blobStorageService.CopyBlobToAsync(
                    requestBody.SourceURL.Split("/")[3], 
                    sourceFileName, 
                    requestBody.DestinationContainer, 
                    destinationFileName
                    );

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }

        [HttpPost("UploadBlobAsync")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadBlobAsync([FromBody] UploadBlobPostRequest requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody.ContainerName) || string.IsNullOrEmpty(requestBody.BlobName))
                {
                    throw new BadHttpRequestException("ContainerName/BlobName cannot be null or empty");
                }

                using Stream readStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody.BlobContent));
                string response = await _blobStorageService.UploadBlobToAsync(requestBody.ContainerName, requestBody.BlobName, readStream, requestBody.Metadata);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal Server Error", Details = ex.Message});
            }
            
        }
    }
}

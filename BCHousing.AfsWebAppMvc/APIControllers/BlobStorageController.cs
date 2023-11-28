using BCHousing.AfsWebAppMvc.Controllers;
using BCHousing.AfsWebAppMvc.Entities.BlobStorage;
using BCHousing.AfsWebAppMvc.Servives.BlobStorageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
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
                string[] UrlParts = url.Split("/");
                string blobName = UrlParts.Length > 5 ? $"{UrlParts[^2]}/{UrlParts[^1]}" : $"{UrlParts[^1]}";
                string containerName = UrlParts[3];
                string response = JsonSerializer.Serialize(await _blobStorageService.GetMetaDataAsync(containerName, blobName));

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
                string response = "";
                string[] UrlParts = requestBody.URL.Split("/");
                string blobName = UrlParts.Length > 5 ? $"{UrlParts[^2]}/{UrlParts[^1]}" : $"{UrlParts[^1]}";
                string containerName = UrlParts[3];
                if (await _blobStorageService.WriteMetaDataAsync(containerName, blobName, metadata: requestBody.Metadata))
                {
                    response = "Successfully update metadata";
                }
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
                //Sample URL: https://afspocstorage.blob.core.windows.net/staging-container/TestNote.txt
                string response = "";
                string destinationFileName = "";
                string sourceFileName = requestBody.SourceURL.Split("/")[^1];

                if (!string.IsNullOrEmpty(requestBody.DestinationFolder)) destinationFileName = $"{requestBody.DestinationFolder}/{sourceFileName}";

                //Call Blobstorage service
                response = await _blobStorageService.CopyBlobToAsync(requestBody.SourceURL.Split("/")[^2], sourceFileName, requestBody.DestinationContainer, destinationFileName);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }
    }
}

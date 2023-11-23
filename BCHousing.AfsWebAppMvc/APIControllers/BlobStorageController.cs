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

        public BlobStorageController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        // GET: api/<BlobStorageAPIController>
        [HttpGet("Metadata")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Metadata([FromBody] MetadataGetRequest requestBody)
        {
            try
            {
                string response = "";
                string[] UrlParts = requestBody.URL.Split("/");
                response = JsonSerializer.Serialize(await _blobStorageService.GetMetaDataAsync(UrlParts[^3], UrlParts[^2] + "/" + UrlParts[^1]));

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
                if (await _blobStorageService.WriteMetaDataAsync(UrlParts[^3], UrlParts[^2] + "/" + UrlParts[^1], metadata: requestBody.Metadata))
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

        // POST api/<BlobStorageAPIController>/StagingBlob/{resource}
        [HttpPost("MoveStagingBlob/{resource}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MoveStagingBlob(string resource, [FromBody] CopyBlobPostRequest requestBody)
        {
            try
            {
                string response = "";
                string fileName = resource;
                if (!string.IsNullOrEmpty(requestBody.DestinationFolder)) fileName = $"{requestBody.DestinationFolder}/{fileName}";

                //Call Blobstorage service
                if(await _blobStorageService.CopyBlobToAsync("staging-container", resource, requestBody.DestinationContainer, fileName)){
                    response = $"Successfully move the file to {requestBody.DestinationContainer}/{requestBody.DestinationFolder}";
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }
    }
}

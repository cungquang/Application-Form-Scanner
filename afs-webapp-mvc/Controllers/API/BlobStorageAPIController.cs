using afs_webapp_mvc.Services.BlobStorageService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace afs_webapp_mvc.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageAPIController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;

        public BlobStorageAPIController(BlobStorageService blobStorageService)
        {
            this._blobStorageService = blobStorageService;
        }

        // GET: api/<BlobStorageAPIController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BlobStorageAPIController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BlobStorageAPIController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BlobStorageAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BlobStorageAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

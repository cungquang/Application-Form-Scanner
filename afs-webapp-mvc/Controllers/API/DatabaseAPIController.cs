using afs_webapp_mvc.Services.AfsDbContextService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace afs_webapp_mvc.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseAPIController : ControllerBase
    {
        private readonly AfsDbContextService _databaseEngine;

        public DatabaseAPIController(AfsDbContextService afsDbEngine) 
        {
            _databaseEngine = afsDbEngine;
        }

        // GET: api/<DatabaseAPIController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DatabaseAPIController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DatabaseAPIController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DatabaseAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DatabaseAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

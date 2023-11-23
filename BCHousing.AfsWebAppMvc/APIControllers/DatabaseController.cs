using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Repositories;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BCHousing.AfsWebAppMvc.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {

        private readonly ISubmissionLogRepository _submissionLogRepository;

        public DatabaseController(ISubmissionLogRepository submissionLogRepository)
        {
            _submissionLogRepository = submissionLogRepository;
        }

        // GET: api/Database/GetSubmissionForms
        [HttpGet("GetSubmissionForms")]
        public async Task<IList<SubmissionLog>> GetSubmissionForms()
        {
            IList<SubmissionLog> submissionForms = await _submissionLogRepository.GetSubmissionLogs();

            return submissionForms;
        }

        // GET: api/Database/CreateSubmissionLogRecord?classifyType=<classify type>&fileUrl=<file url>
        [HttpGet("CreateNewSubmissionLog")]
        public async Task<string> CreateSubmissionLogRecord([FromQuery] string classifyType, [FromQuery] string fileUrl)
        {
            // TODO: Blob API will return a proper metadata
            SubmissionLog newLog = new SubmissionLog();
            newLog.submissionId = Guid.NewGuid();
            newLog.timestamp = DateTime.Now;
            newLog.submit_by = "Mary Cooper";
            newLog.document_name = "Ptt123xyz.pdf";
            newLog.document_size = 89574;
            newLog.user_declared_type = "SAFER";
            newLog.classify_type = classifyType;
            newLog.is_read = true;
            newLog.avg_confidence_level = (decimal)0.75;
            newLog.path_to_document = fileUrl;
            newLog.path_to_analysis_report = "path/Ptt123xyz.pdf";


            int result = await _submissionLogRepository.CreateSubmissionLog(newLog);
            string resultMessage = "Insertion Failed";
            if (result == 1) {
                resultMessage = "Insertion Successfully";
            }

            return resultMessage;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////        Default Methods           //////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        
        [HttpGet]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DatabaseAPIController>/5
        [HttpGet("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DatabaseAPIController>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DatabaseAPIController>/5
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DatabaseAPIController>/5
        [HttpDelete("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void Delete(int id)
        {
        }
    }
}

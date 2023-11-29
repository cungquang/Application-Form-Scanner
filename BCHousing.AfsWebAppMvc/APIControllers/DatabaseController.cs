using BCHousing.AfsWebAppMvc.Entities;
using BCHousing.AfsWebAppMvc.Entities.Database;
using BCHousing.AfsWebAppMvc.Servives.AfsDatabaseService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BCHousing.AfsWebAppMvc.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IAfsDatabaseService _afsDatabaseService;

        public DatabaseController(IAfsDatabaseService afsDatabaseService)
        {
            _afsDatabaseService = afsDatabaseService;
        }

        // GET: api/Database/GetSubmissionLogs
        [HttpGet("GetSubmissionLogs")]
        public async Task<IList<SubmissionLog>> GetSubmissionLogs()
        {
            return await _afsDatabaseService.GetAllSubmissionLogsSync();
        }

        // GET: api/DataBase/GetSubmissionLogByUrl?fileUrl=<file Url>
        [HttpGet("GetSubmissionLogByUrl")]
        public async Task<SubmissionLog> GetSubmissionLogByUrl([FromQuery] string fileUrl)
        {
            return await _afsDatabaseService.GetSubmissionLogByUrl(fileUrl);
        }


        // POST: api/Database/CreateSubmissionLog
        [HttpPost("CreateSubmissionLog")]
        public async Task<string> CreateSubmissionLog([FromBody] SubmissionLogRequest requestBody)
        {
            return await _afsDatabaseService.CreateSubmissionLog(requestBody);
        }

        // POST: api/Database/CreateFormRecord
        [HttpPost("CreateFormRecord")]
        public async Task<FormRecordRequest> CreateFormRecord([FromBody] FormRecordRequest requestBody)
        { 
            return await _afsDatabaseService.CreateFormRecord(requestBody);
        }

        // PUT: api/Database/UpdatePathToFile
        [HttpPut("UpdatePathToFile")]
        public async Task<string> UpdatePathToFile([FromBody] UpdateFilePath requestBody) {
            return await _afsDatabaseService.UpdatePathToFile(requestBody);
        }

        // PUT: api/Database/UpdateSubmissionLogAfterOCRExtraction
        [HttpPut("UpdateSubmissionLogAfterOCRExtraction")]
        public async Task<string> UpdateSubmissionLogAfterOCRExtraction([FromBody] UpdateLogAfterOCRExtraction requestBody)
        {
            return await _afsDatabaseService.UpdateSubmissionLogAfterOCRExtraction(requestBody);
        }
    }
}

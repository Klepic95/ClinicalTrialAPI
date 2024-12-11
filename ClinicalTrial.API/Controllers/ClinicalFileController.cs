using ClinicalTrial.Proxy.Interfaces;
using ClinicalTrial.Proxy.Models;
using ClinicalTrial.Proxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalFileController : ControllerBase
    {
        private readonly IClinicalTrialService _clinicalTrialService;
        private readonly ILogger<ClinicalFileController> _logger;

        public ClinicalFileController(IClinicalTrialService clinicalTrialService, ILogger<ClinicalFileController> logger)
        {
            _clinicalTrialService = clinicalTrialService;
            _logger = logger;
        }

        [HttpPost("uploadFile")]
        public async Task<ActionResult<ProcessClinicalFile>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || Path.GetExtension(file.FileName) != ".json")
            {
                _logger.LogError("Invalid file type.");
                return BadRequest("Invalid file type.");
            }

            var result = await _clinicalTrialService.ProcessFileAsync(file);
            if (!result.IsSuccess)
            {
                _logger.LogError(result.Message, "Result did not indicate success. Please investigate provided error message.");
                return BadRequest(result.Message);
            }

            return Ok("File processed successfully.");
        }
    }
}

using ClinicalTrial.Proxy.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalFileController : ControllerBase
    {
        private readonly IClinicalTrialService _clinicalTrialService;

        public ClinicalFileController(IClinicalTrialService clinicalTrialService)
        {
            _clinicalTrialService = clinicalTrialService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || Path.GetExtension(file.FileName) != ".json")
            {
                return BadRequest("Invalid file type.");
            }

            if (file.Length > 10 * 1024 * 1024) // Limit to 10MB
            {
                return BadRequest("File size exceeds limit.");
            }

            var result = await _clinicalTrialService.ProcessFileAsync(file);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok("File processed successfully.");
        }
    }
}

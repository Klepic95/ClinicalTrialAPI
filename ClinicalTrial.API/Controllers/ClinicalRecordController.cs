using ClinicalTrial.Proxy.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalRecordController : ControllerBase
    {
        private readonly IClinicalTrialService _clinicalTrialService;
        private readonly ILogger<ClinicalRecordController> _logger;

        public ClinicalRecordController(IClinicalTrialService clinicalTrialService, ILogger<ClinicalRecordController> logger)
        {
            _clinicalTrialService = clinicalTrialService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var trial = await _clinicalTrialService.GetClinicalTrialByIdAsync(id);
            if (trial == null)
            {
                _logger.LogError("Clinical record with the provided id does not exist.");
                return NotFound("Clinical record with the provided id does not exist.");
            }
            return Ok(trial);
        }
    }
}

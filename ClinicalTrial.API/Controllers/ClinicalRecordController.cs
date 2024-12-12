﻿using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Models;
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
        public async Task<ActionResult<ClinicalRecordDTO>> GetById(Guid id)
        {
            var trial = await _clinicalTrialService.GetClinicalTrialByIdAsync(id);
            if (trial == null)
            {
                _logger.LogError("Clinical record with the provided id does not exist.");
                return NotFound("Clinical record with the provided id does not exist.");
            }
            return Ok(trial);
        }

        [HttpGet("filterClinicalTrials")]
        public async Task<ActionResult<IEnumerable<ClinicalRecordDTO>>> FilterClinicalTrials([FromQuery]string? status, [FromQuery]int? minParticipants, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate)
        {
            try
            {
                var trials = await _clinicalTrialService.GetFilteredTrialsAsync(status, minParticipants, startDate, endDate);
                if (!trials.Any())
                {
                    _logger.LogWarning("No trials found matching the given criteria.");
                    return NotFound("No trials found matching the given criteria.");
                }

                return Ok(trials);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occured. Please check exception for more details.");
                throw;
            }
        }
    }
}

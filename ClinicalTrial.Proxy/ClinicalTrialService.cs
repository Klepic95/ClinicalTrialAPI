using ClinicalTrial.DAL.Context;
using ClinicalTrial.Proxy.Interfaces;
using ClinicalTrial.Proxy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ClinicalTrial.Proxy.Services;
public class ClinicalTrialService : IClinicalTrialService
{
    private readonly ClinicalTrialDbContext _dbContext;
    private readonly ILogger<ClinicalTrialService> _logger;

    public ClinicalTrialService(ClinicalTrialDbContext dbContext, ILogger<ClinicalTrialService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ProcessClinicalFile> ProcessFileAsync(IFormFile file)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var jsonContent = await reader.ReadToEndAsync();

            // Validate the JSON with schema here
            // Assume 'ValidateJson' is a helper method to validate the JSON schema
            if (!ValidateJson(jsonContent))
            {
                return ProcessClinicalFile.Failure("Invalid JSON schema.");
            }

            // Map JSON to ClinicalTrial
            var trials = JsonConvert.DeserializeObject<List<DAL.Models.ClinicalTrial>>(jsonContent);
            foreach (var trial in trials)
            {
                trial.DurationInDays = (trial.EndDate - trial.StartDate).Days;
                if (trial.Status == "Ongoing" && trial.EndDate == default)
                {
                    trial.EndDate = trial.StartDate.AddMonths(1);
                }

                _dbContext.ClinicalTrials.Add(trial);
            }

            await _dbContext.SaveChangesAsync();
            return ProcessClinicalFile.Success("File processed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file.");
            return ProcessClinicalFile.Failure("Error processing file.");
        }
    }

    private bool ValidateJson(string jsonContent)
    {
        // This will be implemented later with the provided schema
        return true;
    }
}

using ClinicalTrial.DAL;
using ClinicalTrial.DAL.Interfaces;
using ClinicalTrial.DAL.Models;
using ClinicalTrial.Proxy.Interfaces;
using ClinicalTrial.Proxy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ClinicalTrial.Proxy.Services
{
    public class ClinicalTrialService : IClinicalTrialService
    {
        private readonly IClinicalTrialRepository _repository;
        private readonly ILogger<ClinicalTrialService> _logger;
        private readonly string _schemaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TemplateFileValidation.json");


        public ClinicalTrialService(IClinicalTrialRepository repository, ILogger<ClinicalTrialService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ProcessClinicalFile> ProcessFileAsync(IFormFile file)
        {
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var jsonContent = await reader.ReadToEndAsync();

                // JSON will first be validated against a schema before proceeding
                if (!ValidateJson(jsonContent))
                {
                    return ProcessClinicalFile.Failure("Invalid JSON schema.");
                }

                var clinicalTrial = JsonConvert.DeserializeObject<Models.ClinicalTrial>(jsonContent);
                var clinicalTialDTO = TransformToDTOModel(clinicalTrial);

                if (clinicalTialDTO.Status == "Ongoing" && clinicalTialDTO.EndDate == default)
                {
                    clinicalTialDTO.EndDate = clinicalTialDTO.StartDate.AddMonths(1);
                }
                clinicalTialDTO.DurationInDays = (clinicalTialDTO.EndDate - clinicalTialDTO.StartDate).Days;

                await _repository.AddClinicalTrialAsync(clinicalTialDTO);
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
            try
            {
                var schemaJson = GetSchemaJson();
                var schema = JSchema.Parse(schemaJson);

                var json = JObject.Parse(jsonContent);

                var isValid = json.IsValid(schema, out IList<string> errorMessages);

                if (!isValid)
                {
                    foreach (var errorMessage in errorMessages)
                    {
                        _logger.LogError("JSON Schema Validation Error: " + errorMessage);
                    }
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JSON schema.");
                return false;
            }
        }

        private string GetSchemaJson()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(_schemaFilePath))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private ClinicalTrialDTO TransformToDTOModel(Models.ClinicalTrial trial)
        {
            return new ClinicalTrialDTO
            {
                TrialId = trial.TrialId,
                Title = trial.Title,
                StartDate = trial.StartDate,
                EndDate = trial.EndDate,
                Participants = trial.Participants,
                Status = trial.Status.ToString()
            };
        }

        private Models.ClinicalTrial TransformToProxyModel(ClinicalTrialDTO trialDTO)
        {
            return new Models.ClinicalTrial
            {
                TrialId = trialDTO.TrialId,
                Title = trialDTO.Title,
                StartDate = trialDTO.StartDate,
                EndDate = trialDTO.EndDate,
                Participants = trialDTO.Participants,
                Status = Enum.TryParse<TrialStatus>(trialDTO.Status, out var status) ? status : TrialStatus.Ongoing,
            };
        }
    }
}

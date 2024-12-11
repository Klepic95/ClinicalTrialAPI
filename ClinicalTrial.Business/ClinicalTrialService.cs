using ClinicalTrial.DAL;
using ClinicalTrial.DAL.Interfaces;
using ClinicalTrial.DAL.Models;
using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ClinicalTrial.Business.Services
{
    public class ClinicalTrialService : IClinicalTrialService
    {
        private readonly IClinicalTrialRepository _repository;
        private readonly ILogger<ClinicalTrialService> _logger;
        private readonly string _schemaFilePath = Assembly.GetExecutingAssembly().GetManifestResourceNames()[0];


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

        public async Task<ClinicalRecord> GetClinicalTrialByIdAsync(Guid id)
        {
            try
            {
                var clinicalTrialDTO = await _repository.GetClinicialTrialByIdAsync(id);
                return TransformToRepresentationModel(clinicalTrialDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Clinical record not found!");
                throw new KeyNotFoundException("Clinical record not found!");
            }
        }

        public async Task<IEnumerable<ClinicalRecord>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var validStatuses = new[] { "NotStarted", "Ongoing", "Completed" };
            if (!string.IsNullOrEmpty(status) && !validStatuses.Contains(status))
            {
                _logger.LogError("Invalid status provided. Please provide a valid status.");
                throw new InvalidDataException("Invalid status provided. Please provide a valid status.");
            }
            if(minParticipants.HasValue && minParticipants <= 0)
            {
                _logger.LogError("Invalid number of participants provided. Please provide a valid number of participants.");
                throw new InvalidDataException("Invalid number of participants provided. Please provide a valid number of participants.");
            }
            if(startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                _logger.LogError("Start date cannot be greater than end date.");
                throw new InvalidDataException("Start date cannot be greater than end date");
            }

            var clinicalTrialDTOs = await _repository.GetFilteredTrialsAsync(status, minParticipants, startDate, endDate);
            var clinicalRecords = new List<ClinicalRecord>();
            if (clinicalTrialDTOs.Count() == 0)
            {
                _logger.LogWarning("No clinical records found for the given filter criteria.");
                return clinicalRecords;
            }
            foreach (var clinicalTrialDto in clinicalTrialDTOs)
            {
                clinicalRecords.Add(TransformToRepresentationModel(clinicalTrialDto));
            }
            return clinicalRecords;
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

        private ClinicalRecord TransformToRepresentationModel(ClinicalTrialDTO trialDTO)
        {
            return new Models.ClinicalRecord
            {
                TrialId = trialDTO.TrialId,
                Title = trialDTO.Title,
                StartDate = trialDTO.StartDate,
                EndDate = trialDTO.EndDate,
                Participants = trialDTO.Participants,
                Status = Enum.TryParse<TrialStatus>(trialDTO.Status, out var status) ? status : TrialStatus.Ongoing,
                DurationInDays = trialDTO.DurationInDays
            };
        }
    }
}

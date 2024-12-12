using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Reflection;

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

        public async Task<ProcessClinicalFileDTO> ProcessFileAsync(IFormFile file)
        {
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var jsonContent = await reader.ReadToEndAsync();

                // JSON will first be validated against a schema before proceeding
                if (!ValidateJson(jsonContent))
                {
                    return ProcessClinicalFileDTO.Failure("Invalid JSON schema.");
                }

                var clinicalTrialDTO = JsonConvert.DeserializeObject<Models.ClinicalTrialDTO>(jsonContent);

                if(clinicalTrialDTO?.Participants < 1)
                {
                    return ProcessClinicalFileDTO.Failure("Participant number must be greater than 0.");
                }

                var clinicalTial = TransformToDTOModel(clinicalTrialDTO);

                if ((clinicalTial.Status == "Ongoing" && clinicalTial.EndDate == default) ||
                    clinicalTial.EndDate == default)
                {
                    clinicalTial.EndDate = clinicalTial.StartDate.AddMonths(1);
                }
                clinicalTial.DurationInDays = (clinicalTial.EndDate - clinicalTial.StartDate).Days;

                var generatedClinicalTrialId = await _repository.AddClinicalTrialAsync(clinicalTial);
                return ProcessClinicalFileDTO.Success("File processed successfully.", generatedClinicalTrialId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file.");
                return ProcessClinicalFileDTO.Failure("Error processing file.");
            }
        }

        public async Task<ClinicalRecordDTO> GetClinicalTrialByIdAsync(Guid id)
        {
            try
            {
                var clinicalTrial = await _repository.GetClinicialTrialByIdAsync(id);
                if (clinicalTrial == null)
                {
                    _logger.LogError("Clinical record not found!");
                    throw new KeyNotFoundException("Clinical record not found!");
                }

                return TransformToRepresentationModel(clinicalTrial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to fetch specific record.");
                throw;
            }
        }

        public async Task<IEnumerable<ClinicalRecordDTO>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null)
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
            var clinicalRecordDTOs = new List<ClinicalRecordDTO>();
            if (clinicalTrialDTOs.Count() == 0)
            {
                _logger.LogWarning("No clinical records found for the given filter criteria.");
                return clinicalRecordDTOs;
            }
            foreach (var clinicalTrialDTO in clinicalTrialDTOs)
            {
                clinicalRecordDTOs.Add(TransformToRepresentationModel(clinicalTrialDTO));
            }
            return clinicalRecordDTOs;
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

        private Models.ClinicalTrial TransformToDTOModel(Models.ClinicalTrialDTO trialclinicalTrialDTO)
        {
            return new Models.ClinicalTrial
            {
                TrialId = trialclinicalTrialDTO.TrialId,
                Title = trialclinicalTrialDTO.Title,
                StartDate = trialclinicalTrialDTO.StartDate,
                EndDate = trialclinicalTrialDTO.EndDate,
                Participants = trialclinicalTrialDTO.Participants,
                Status = trialclinicalTrialDTO.Status.ToString()
            };
        }

        private ClinicalRecordDTO TransformToRepresentationModel(Models.ClinicalTrial clinicalTrial)
        {
            return new Models.ClinicalRecordDTO
            {
                TrialId = clinicalTrial.TrialId,
                Title = clinicalTrial.Title,
                StartDate = clinicalTrial.StartDate,
                EndDate = clinicalTrial.EndDate,
                Participants = clinicalTrial.Participants,
                Status = Enum.TryParse<TrialStatus>(clinicalTrial.Status, out var status) ? status : TrialStatus.Ongoing,
                DurationInDays = clinicalTrial.DurationInDays
            };
        }
    }
}

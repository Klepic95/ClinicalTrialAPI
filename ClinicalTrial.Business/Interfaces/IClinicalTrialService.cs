using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Http;

namespace ClinicalTrial.Business.Interfaces
{
    public interface IClinicalTrialService
    {
        Task<ProcessClinicalFileDTO> ProcessFileAsync(IFormFile file);
        Task<ClinicalRecordDTO> GetClinicalTrialByIdAsync(Guid id);
        Task<IEnumerable<ClinicalRecordDTO>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}

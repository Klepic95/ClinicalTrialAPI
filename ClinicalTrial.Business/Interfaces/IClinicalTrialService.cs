using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Http;

namespace ClinicalTrial.Business.Interfaces
{
    public interface IClinicalTrialService
    {
        Task<ProcessClinicalFile> ProcessFileAsync(IFormFile file);
        Task<ClinicalRecord> GetClinicalTrialByIdAsync(Guid id);
        Task<IEnumerable<ClinicalRecord>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}

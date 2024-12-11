using ClinicalTrial.DAL.Models;

namespace ClinicalTrial.DAL.Interfaces
{
    public interface IClinicalTrialRepository
    {
        Task<Guid> AddClinicalTrialAsync(ClinicalTrialDTO trialDTO);
        Task<ClinicalTrialDTO> GetClinicialTrialByIdAsync(Guid id);
        Task<IEnumerable<ClinicalTrialDTO>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}

using ClinicalTrial.DAL.Models;

namespace ClinicalTrial.DAL.Interfaces
{
    public interface IClinicalTrialRepository
    {
        Task AddClinicalTrialAsync(ClinicalTrialDTO trialDTO);
        Task<ClinicalTrialDTO> GetByTrialIdAsync(Guid id);
    }
}

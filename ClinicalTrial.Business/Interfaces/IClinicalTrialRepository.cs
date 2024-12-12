namespace ClinicalTrial.Business.Interfaces
{
    public interface IClinicalTrialRepository
    {
        Task<Guid> AddClinicalTrialAsync(Models.ClinicalTrial trialDTO);
        Task<Models.ClinicalTrial> GetClinicialTrialByIdAsync(Guid id);
        Task<IEnumerable<Models.ClinicalTrial>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}

using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrial.DAL
{
    public class ClinicalTrialRepository : IClinicalTrialRepository
    {
        private readonly ClinicalTrialDbContext _dbContext;

        public ClinicalTrialRepository(ClinicalTrialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddClinicalTrialAsync(Business.Models.ClinicalTrial trialDTO)
        {
            _dbContext.ClinicalTrials.Add(trialDTO);
            await _dbContext.SaveChangesAsync();
            return trialDTO.Id;
        }

        public async Task<Business.Models.ClinicalTrial> GetClinicialTrialByIdAsync(Guid id)
        {
            return await _dbContext.ClinicalTrials
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Business.Models.ClinicalTrial>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.ClinicalTrials.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }
            if (minParticipants.HasValue)
            {
                query = query.Where(t => t.Participants >= minParticipants.Value);
            }
            if (startDate.HasValue)
            {
                query = query.Where(t => t.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(t => t.EndDate <= endDate.Value);
            }

            return await query.ToListAsync();
        }
    }
}

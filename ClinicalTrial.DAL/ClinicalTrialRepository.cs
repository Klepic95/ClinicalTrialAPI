using ClinicalTrial.DAL.Context;
using ClinicalTrial.DAL.Interfaces;
using ClinicalTrial.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalTrial.DAL
{
    public class ClinicalTrialRepository : IClinicalTrialRepository
    {
        private readonly ClinicalTrialDbContext _dbContext;

        public ClinicalTrialRepository(ClinicalTrialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddClinicalTrialAsync(ClinicalTrialDTO trialDTO)
        {
            _dbContext.ClinicalTrials.Add(trialDTO);
            await _dbContext.SaveChangesAsync();
            return trialDTO.Id;
        }

        public async Task<ClinicalTrialDTO> GetClinicialTrialByIdAsync(Guid id)
        {
            return await _dbContext.ClinicalTrials
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClinicalTrialDTO>> GetFilteredTrialsAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null)
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

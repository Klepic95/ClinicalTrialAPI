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

        public async Task AddClinicalTrialAsync(ClinicalTrialDTO trialDTO)
        {
            _dbContext.ClinicalTrials.Add(trialDTO);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ClinicalTrialDTO> GetClinicialTrialByIdAsync(Guid id)
        {
            return await _dbContext.ClinicalTrials
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ClinicalTrial.DAL.Context;

public class ClinicalTrialDbContext : DbContext
{
    public ClinicalTrialDbContext(DbContextOptions<ClinicalTrialDbContext> options) : base(options) { }

    public DbSet<Models.ClinicalTrialDTO> ClinicalTrials { get; set; }
}
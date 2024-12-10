using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ClinicalTrial.DAL.Context;

public class ClinicalTrialDbContext : DbContext
{
    public ClinicalTrialDbContext(DbContextOptions<ClinicalTrialDbContext> options) : base(options) { }

    public DbSet<Models.ClinicalTrialDTO> ClinicalTrials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.ClinicalTrialDTO>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Models.ClinicalTrialDTO>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
    }
}
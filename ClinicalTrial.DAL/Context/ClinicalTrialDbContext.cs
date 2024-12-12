using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrial.DAL.Context;

public class ClinicalTrialDbContext : DbContext
{
    public DbSet<Business.Models.ClinicalTrial> ClinicalTrials { get; set; }

    public ClinicalTrialDbContext()
    {
            
    }
    public ClinicalTrialDbContext(DbContextOptions<ClinicalTrialDbContext> options) : base(options) 
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionstring());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Business.Models.ClinicalTrial>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Business.Models.ClinicalTrial>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
    }

    private static string GetConnectionstring()
    {
        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder()
        {
            DataSource = isDocker ? "host.docker.internal,1433" : "localhost,1433",
            UserID = "sa",
            Password = "ctaPass1!",
            TrustServerCertificate = true,
        };
        return builder.ConnectionString;
    }
}
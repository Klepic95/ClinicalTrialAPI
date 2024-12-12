using ClinicalTrial.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalTrial.DAL.Extensions
{
    public static class DalExtensions
    {
        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddScoped<IClinicalTrialRepository, ClinicalTrialRepository>();
            return services;
        }
    }
}

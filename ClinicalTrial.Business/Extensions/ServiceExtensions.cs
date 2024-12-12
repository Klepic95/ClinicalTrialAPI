using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalTrial.Business.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClinicalTrialService, ClinicalTrialService>();
            return services;
        }
    }
}

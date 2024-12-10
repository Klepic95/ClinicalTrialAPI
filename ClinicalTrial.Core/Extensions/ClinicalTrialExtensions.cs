using ClinicalTrial.DAL;
using ClinicalTrial.DAL.Context;
using ClinicalTrial.DAL.Interfaces;
using ClinicalTrial.Proxy.Interfaces;
using ClinicalTrial.Proxy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinical.Core.Extensions
{
    public static class ClinicalTrialExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClinicalTrialService, ClinicalTrialService>();
            return services;
        }

        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddScoped<IClinicalTrialRepository, ClinicalTrialRepository>();
            return services;
        }

        public static IServiceCollection AddClinicalTrialDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ClinicalTrialDbContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }
    }
}

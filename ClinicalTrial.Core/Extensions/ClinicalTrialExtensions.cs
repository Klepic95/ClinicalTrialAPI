using ClinicalTrial.DAL.Context;
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
            // Register your services
            services.AddScoped<IClinicalTrialService, ClinicalTrialService>();

            return services;
        }

        public static IServiceCollection AddClinicalTrialDbContext(this IServiceCollection services, string connectionString)
        {
            // Register DbContext
            services.AddDbContext<ClinicalTrialDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}

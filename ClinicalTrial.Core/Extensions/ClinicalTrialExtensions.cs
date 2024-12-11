using ClinicalTrial.DAL;
using ClinicalTrial.DAL.Context;
using ClinicalTrial.DAL.Interfaces;
using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

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

        public static IServiceCollection AddClinicalTrialSwaggerExtension(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Add Swagger/OpenAPI services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Clinical Trial API",
                    Version = "v1",
                    Description = "An API for managing clinical trials.",
                });
            });
            return services;
        }
    }
}

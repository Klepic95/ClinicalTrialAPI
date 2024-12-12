using ClinicalTrial.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace ClinicalTrial.API.Extensions
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddClinicalTrialDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ClinicalTrialDbContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }

        public static IServiceCollection AddClinicalTrialSwaggerExtension(this IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new TrialStatusConverter());
                });
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

        public static IServiceCollection ConfigureCORSPolicy(this IServiceCollection services)
        {
            // For the testing purposes we allow all origins, methods and headers
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            return services;
        }
    }
}

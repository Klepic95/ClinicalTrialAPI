using ClinicalTrial.API.Extensions;
using ClinicalTrial.Business.Extensions;
using ClinicalTrial.DAL.Extensions;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddClinicalTrialDbContext(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddApplicationServices();
builder.Services.AddApplicationRepositories();
builder.Services.AddClinicalTrialSwaggerExtension();
builder.Services.ConfigureCORSPolicy();

builder.Services.Configure<FormOptions>(options =>
{
    // This will set maximum file size constraints to 10MB
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinical Trial API");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

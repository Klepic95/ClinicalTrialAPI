using ClinicalTrial.Proxy.Models;
using Microsoft.AspNetCore.Http;

namespace ClinicalTrial.Proxy.Interfaces
{
    public interface IClinicalTrialService
    {
        Task<ProcessClinicalFile> ProcessFileAsync(IFormFile file);
    }
}

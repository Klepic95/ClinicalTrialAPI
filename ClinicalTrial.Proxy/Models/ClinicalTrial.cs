using ClinicalTrial.Proxy.Models;
using System.Text.Json.Serialization;

namespace ClinicalTrial.Proxy.Models
{
    public class ClinicalTrial
    {
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Participants { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrialStatus Status { get; set; }
    }
}

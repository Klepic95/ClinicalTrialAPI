using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClinicalTrial.Proxy.Models
{
    public class ClinicalRecord
    {
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Participants { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrialStatus Status { get; set; }
        public int DurationInDays { get; set; }
    }
}

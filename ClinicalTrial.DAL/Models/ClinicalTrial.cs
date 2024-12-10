namespace ClinicalTrial.DAL.Models
{
    public class ClinicalTrial
    {
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Participants { get; set; }
        public string Status { get; set; }
        // Per the requirements, the duration of the trial is calculated
        public int DurationInDays { get; set; }
    }
}

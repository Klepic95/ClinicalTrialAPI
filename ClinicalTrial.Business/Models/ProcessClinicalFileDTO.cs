namespace ClinicalTrial.Business.Models
{
    public class ProcessClinicalFileDTO
    {
        public Guid Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ProcessClinicalFileDTO Success(string message, Guid id)
        {
            return new ProcessClinicalFileDTO { IsSuccess = true, Message = message, Id = id };
        }

        public static ProcessClinicalFileDTO Failure(string message)
        {
            return new ProcessClinicalFileDTO { IsSuccess = false, Message = message };
        }
    }
}

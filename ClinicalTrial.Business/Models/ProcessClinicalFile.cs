namespace ClinicalTrial.Business.Models
{
    public class ProcessClinicalFile
    {
        public Guid Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ProcessClinicalFile Success(string message, Guid id)
        {
            return new ProcessClinicalFile { IsSuccess = true, Message = message, Id = id };
        }

        public static ProcessClinicalFile Failure(string message)
        {
            return new ProcessClinicalFile { IsSuccess = false, Message = message };
        }
    }
}

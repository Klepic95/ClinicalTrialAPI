namespace ClinicalTrial.Business.Models
{
    public class ProcessClinicalFile
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ProcessClinicalFile Success(string message)
        {
            return new ProcessClinicalFile { IsSuccess = true, Message = message };
        }

        public static ProcessClinicalFile Failure(string message)
        {
            return new ProcessClinicalFile { IsSuccess = false, Message = message };
        }
    }
}

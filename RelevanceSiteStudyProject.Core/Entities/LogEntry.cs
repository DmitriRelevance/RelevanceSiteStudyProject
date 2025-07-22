using System.ComponentModel.DataAnnotations;

namespace RelevanceSiteStudyProject.Data
{
    public class LogEntry
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string StackTrace { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
    }
}

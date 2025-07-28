using System.ComponentModel.DataAnnotations;

namespace RelevanceSiteStudyProject.Core.Entities
{
    public class LogEntry
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; } = string.Empty;
        [Required]
        public string StackTrace { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
    }
}

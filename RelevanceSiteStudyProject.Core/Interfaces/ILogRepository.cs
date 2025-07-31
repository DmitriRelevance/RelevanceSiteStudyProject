using RelevanceSiteStudyProject.Core.Entities;

namespace RelevanceSiteStudyProject.Core.Interfaces
{
    public interface ILogRepository
    {
        Task LogAsync(LogEntry entry);
    }

}

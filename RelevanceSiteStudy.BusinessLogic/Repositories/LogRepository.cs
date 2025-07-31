using RelevanceSiteStudyProject.Core.Entities;
using RelevanceSiteStudyProject.Core.Interfaces;
using RelevanceSiteStudyProject.Infrasactructure.Data;

namespace RelevanceSiteStudy.Services.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly AppDbContext _context;

        public LogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(LogEntry entry)
        {
            _context.LogEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
    }

}

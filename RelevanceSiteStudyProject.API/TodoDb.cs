using Microsoft.EntityFrameworkCore;
using RelevanceSiteStudyProject.API.Models;

namespace RelevanceSiteStudyProject.API
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options)
            : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}

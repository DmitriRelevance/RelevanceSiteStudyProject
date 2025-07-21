using RelevanceSiteStudyProject.Data;

namespace RelevanceSiteStudyProject.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetPosts()
        {
            return _context.Posts.ToList();
        }
    }

    public interface IPostService
    {
        Task<List<Post>> GetPosts();
    }
}

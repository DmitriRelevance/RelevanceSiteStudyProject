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

        public async Task<Post> Add(Post post)
        {
            try
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return post;
        }
        public async Task<List<Post>> GetPosts()
        {
            return _context.Posts.ToList();
        }
        public Task Update(Post post)
        {
            throw new NotImplementedException();
        }
        public Task Delete(Post post, User currentUser)
        {
            var existingPost = _context.Posts.Find(post.Id);
            if (existingPost != null)
            {
                if (currentUser != null && (existingPost.User.Id == currentUser.Id || currentUser.IsAdmin))
                {
                    _context.Posts.Remove(existingPost);
                    return _context.SaveChangesAsync();
                }
                else
                {
                    throw new UnauthorizedAccessException("You do not have permission to delete this post.");
                }

            }
            else
            {
                throw new KeyNotFoundException("Post not found");
            }
        }
    }

    public interface IPostService
    {
        Task<Post> Add(Post post);
        Task<List<Post>> GetPosts();
        Task Update(Post post);
        Task Delete(Post post, User currentUser);
    }
}

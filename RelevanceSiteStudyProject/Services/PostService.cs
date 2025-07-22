using Microsoft.EntityFrameworkCore;
using RelevanceSiteStudyProject.Interfaces;
using RelevanceSiteStudyProject.Helpers;
using RelevanceSiteStudyProject.ViewModels;

namespace RelevanceSiteStudyProject.Services
{
    public class PostService : IPostService
    {
        private readonly Data.AppDbContext _context;
        private readonly ILogger<PostService> _logger;

        public PostService(Data.AppDbContext context, ILogger<PostService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Post> Add(Post post)
        {
            try
            {
                var mappedPost = MappingExtensions.ToDataModel(post);

                _context.Posts.Add(mappedPost);
                await _context.SaveChangesAsync();
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding post: {ex.Message}");
                _context.LogEntries.Add(new Data.LogEntry
                {
                    Message = $"Error adding post: {ex.Message}",
                    StackTrace = ex?.StackTrace ?? string.Empty,
                    Timestamp = DateTime.UtcNow,
                    Action = nameof(Add),
                });
                throw;
            }

        }
        public async Task<IList<Post>> GetPosts()
        {
            var dbPosts = await _context.Posts.ToListAsync();
            return MappingExtensions.ToViewModel<Data.Post, Post>(dbPosts, MappingExtensions.ToViewModel);
        }
        public async Task Update(Post post, User currentUser)
        {
            try
            {
                var existingPost = _context.Posts.FirstOrDefault(p => p.Id == post.Id);
                if (existingPost is null)
                    throw new KeyNotFoundException("Couldn't find your post!");

                if (currentUser != null && (currentUser.IsAdmin || existingPost.UserId == currentUser.Id))
                {
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new UnauthorizedAccessException("You do not have permission to edit this post.");
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during {nameof(Update)} the post: {ex.Message}");
                _context.LogEntries.Add(new Data.LogEntry
                {
                    Message = $"Error adding post: {ex.Message}",
                    StackTrace = ex?.StackTrace ?? string.Empty,
                    Timestamp = DateTime.UtcNow,
                    Action = nameof(Update),
                });
                throw;
            }
        }
        public async Task Delete(Post post, User currentUser)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if (existingPost is null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            if (currentUser != null && (existingPost.UserId == currentUser.Id || currentUser.IsAdmin))
            {
                _context.Posts.Remove(existingPost);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this post.");
            }
        }
    }
}

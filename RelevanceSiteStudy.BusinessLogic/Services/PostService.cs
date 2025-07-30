using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;
using RelevanceSiteStudyProject.Core.Interfaces;
using RelevanceSiteStudyProject.Core.Mappers;

namespace RelevanceSiteStudyProject.Services.Services
{
    public class PostService : IPostService
    {
        private readonly RelevanceSiteStudyProject.Infrasactructure.Data.AppDbContext _context;
        private readonly ILogger<PostService> _logger;
        private readonly UserManager<User> _userManager;
        public PostService(RelevanceSiteStudyProject.Infrasactructure.Data.AppDbContext context, ILogger<PostService> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<PostDto> Add(PostCreateDto post)
        {
            try
            {
                var postEntity = PostMapper.ToEntity(post);

                var addedPost  = _context.Posts.Add(postEntity);
                await _context.SaveChangesAsync();
                var dtoResult = PostMapper.ToDto(addedPost.Entity);

                return dtoResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding post: {ex.Message}");
                _context.LogEntries.Add(new LogEntry
                {
                    Message = $"Error adding post: {ex.Message}",
                    StackTrace = ex?.StackTrace ?? string.Empty,
                    Timestamp = DateTime.UtcNow,
                    Action = nameof(Add),
                });
                throw;
            }

        }
        public async Task<IList<PostDto>> GetPosts()
         {
            var dbPosts = await _context.Posts.ToListAsync();
            var result = PostMapper.ToDto<Post, PostDto>(dbPosts, p => p.ToDto());
              return result;
        }

        public async Task Update(PostDto dto, string userId)
        {
            var existing = await _context.Posts.FirstOrDefaultAsync(p => p.Id == dto.Id);
            if (existing is null)
                throw new KeyNotFoundException("Couldn't find your post!");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin || existing.UserId == userId)
            {
                existing.Title = dto.Title;
                existing.Content = dto.Content;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException("You cannot edit this post.");
            }
        }
        public async Task Delete(int postId, string userId)
        {
            var existingPost = await _context.Posts.FindAsync(postId);
            if (existingPost is null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            if (user != null && (existingPost.UserId.Equals(user.Id) || user.IsAdmin))
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

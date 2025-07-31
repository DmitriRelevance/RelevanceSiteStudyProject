using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;
using RelevanceSiteStudyProject.Core.Interfaces;
using RelevanceSiteStudyProject.Core.Mappers;

namespace RelevanceSiteStudyProject.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<PostService> _logger;
        private readonly UserManager<User> _userManager;
        public PostService(IPostRepository postRepository, ILogRepository logRepository, ILogger<PostService> logger, UserManager<User> userManager)
        {
            _postRepository = postRepository;
            _logRepository = logRepository;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<PostDto> Add(PostCreateDto post)
        {
            try
            {
                var postEntity = PostMapper.ToEntity(post);

                var addedPost  = await _postRepository.AddAsync(postEntity);
                var dtoResult = PostMapper.ToDto(addedPost);

                return dtoResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding post: {ex.Message}");

                try
                {
                    await _logRepository.LogAsync(new LogEntry
                    {
                        Message = $"Error adding post: {ex.Message}",
                        StackTrace = ex.StackTrace ?? string.Empty,
                        Timestamp = DateTime.UtcNow,
                        Action = nameof(Add)
                    });
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Failed to write log entry to database");
                }

                throw;
            }

        }
        public async Task<IList<PostDto>> GetPosts()
         {
            var dbPosts = await _postRepository.GetAllAsync();
            var result = PostMapper.ToDto<Post, PostDto>(dbPosts, p => p.ToDto());
              return result;
        }

        public async Task Update(PostDto dto, string userId)
        {
            var existing = await _postRepository.GetByIdAsync(dto.Id);
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
                await _postRepository.UpdateAsync(existing);
            }
            else
            {
                throw new UnauthorizedAccessException("You cannot edit this post.");
            }
        }
        public async Task Delete(int postId, string userId)
        {
            var existingPost = await _postRepository.GetByIdAsync(postId);
            if (existingPost is null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            if (user != null && (existingPost.UserId.Equals(user.Id) || user.IsAdmin))
            {
                await _postRepository.DeleteAsync(existingPost);
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this post.");
            }
        }
    }
}

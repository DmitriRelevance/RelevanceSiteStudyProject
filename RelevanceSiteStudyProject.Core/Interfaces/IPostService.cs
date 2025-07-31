using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;

namespace RelevanceSiteStudyProject.Core.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> Add(PostCreateDto post);
        Task<IList<PostDto>> GetPosts();
        Task Update(PostUpdateDto dto, string userId);
        Task Delete(int postId, string userId);
    }
}

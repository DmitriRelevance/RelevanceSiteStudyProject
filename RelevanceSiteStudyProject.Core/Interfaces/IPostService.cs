using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;

namespace RelevanceSiteStudyProject.Core.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> Add(PostDto post);
        Task<IList<PostDto>> GetPosts();
        Task Update(PostDto post, User currentUser);
        Task Delete(PostDto post, User currentUser);
    }
}

using RelevanceSiteStudyProject.Core.Entities;

namespace RelevanceSiteStudyProject.Core.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> AddAsync(Post post);
        Task<IList<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(int id);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
    }
}

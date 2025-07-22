
using RelevanceSiteStudyProject.ViewModels;

namespace RelevanceSiteStudyProject.Interfaces
{
    public interface IPostService
    {
        Task<Post> Add(Post post);
        Task<IList<Post>> GetPosts();
        Task Update(Post post, User currentUser);
        Task Delete(Post post, User currentUser);
    }
}

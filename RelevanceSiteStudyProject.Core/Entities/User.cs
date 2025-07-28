using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RelevanceSiteStudyProject.Core.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public ICollection<Post> Posts { get; internal set; } = new List<Post>();
    }
}

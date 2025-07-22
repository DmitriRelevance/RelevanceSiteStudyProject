using Microsoft.AspNetCore.Identity;

namespace RelevanceSiteStudyProject.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}

using RelevanceSiteStudyProject.Core.Entities;
using System.Security.Claims;

namespace RelevanceSiteStudyProject.Core.Interfaces
{
    public interface IJWTTokenService
    {
        string GenerateJwtToken(User user);
        ClaimsPrincipal? ValidateJwtToken(string token);
    }
}

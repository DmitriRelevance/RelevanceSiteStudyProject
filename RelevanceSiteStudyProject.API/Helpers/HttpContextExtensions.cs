using System.Security.Claims;

namespace RelevanceSiteStudyProject.API.Helpers
{
    public static class HttpContextExtensions
    {
        public static string? TryGetAuthenticatedUserId(HttpContext context)
        {
            var user = context.User;
            if (user.Identity is not { IsAuthenticated: true })
            {
                return null;
            }

            string? result = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return result;
        }
    }
}

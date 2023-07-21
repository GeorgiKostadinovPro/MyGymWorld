namespace MyGymWorld.Web.Infrastructure.Extensions
{
    using System.Security.Claims;

    public static class UserClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            string userId = string.Empty;

            if (user!.Identity!.IsAuthenticated
                && user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userId = user
                    .FindFirstValue(ClaimTypes.NameIdentifier)
                    .ToString();
            }

            return userId;
        }
    }
}

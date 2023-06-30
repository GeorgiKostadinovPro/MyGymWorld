namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [Authorize]
    public class BaseController : Controller
    {
        protected string GetUserId()
        {
            string userId = string.Empty;

            if (this.User!.Identity!.IsAuthenticated
                && this.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userId = this.User
                    .FindFirstValue(ClaimTypes.NameIdentifier)
                    .ToString();
            }

            return userId;
        }
    }
}

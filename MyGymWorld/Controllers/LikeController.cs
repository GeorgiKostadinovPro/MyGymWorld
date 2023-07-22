namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.Infrastructure.Extensions;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService likeService;
        private readonly IGymService gymService;
        private readonly INotificationService notificationService;

        public LikeController(
            ILikeService _likeService, 
            IGymService _gymService, 
            INotificationService _notificationService)
        {
            this.likeService = _likeService;
            this.gymService = _gymService;
            this.notificationService = _notificationService;
        }

        [HttpPost("create/{gymId}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(string gymId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return this.RedirectToAction("Details", "Gym", new { gymId = gymId });
            }

            Gym gym = await this.gymService.GetGymByIdAsync(gymId);

            if (gym == null)
            {
                return this.NotFound();
            }

            try
            {
                string userId = User.GetId();

                await this.likeService.CreateLikeAsync(gymId, userId);

                await this.notificationService.CreateNotificationAsync(
                    $"You liked gym: {gym.Name}",
                    $"/Gym/Details/?gymId={gymId}",
                    userId);

                return this.Ok();
            }
            catch (Exception)
            {
                return this.RedirectToAction("Details", "Gym", new { gymId = gymId });
            }
        }
    }
}

namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.Infrastructure.Extensions;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class LikeController : BaseController
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

        public async Task<IActionResult> Create(string gymId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return this.Unauthorized();
            }

            try
            {   Gym gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    return this.NotFound();
                }

                string userId = this.GetUserId();

                Like like = await this.likeService.CreateLikeAsync(gymId, userId);

                if (like.IsDeleted == false)
                {
                    this.TempData[SuccessMessage] = $"You liked {gym.Name}!";

                    await this.notificationService.CreateNotificationAsync(
                        $"You liked gym: {gym.Name}",
                        $"/Gym/Details/?gymId={gymId}",
                        userId);
                }
                else
                {
                    this.TempData[SuccessMessage] = $"You unliked {gym.Name}!";

                    await this.notificationService.CreateNotificationAsync(
                        $"You unliked gym: {gym.Name}",
                        $"/Gym/Details/?gymId={gymId}",
                        userId);
                } 
                
                return this.RedirectToAction("Details", "Gym", new { gymId = gymId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home");
            }
        }
    }
}
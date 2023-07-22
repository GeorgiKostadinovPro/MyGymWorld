﻿namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class DislikeController : BaseController
    {
        private readonly IDislikeService dislikeService;
        private readonly IGymService gymService;
        private readonly INotificationService notificationService;

        public DislikeController(
            IDislikeService _dislikeService,
            IGymService _gymService,
            INotificationService _notificationService)
        {
            this.dislikeService = _dislikeService;
            this.gymService = _gymService;
            this.notificationService = _notificationService;
        }

        public async Task<IActionResult> Create(string gymId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return this.Unauthorized();
            }

            Gym gym = await this.gymService.GetGymByIdAsync(gymId);

            if (gym == null)
            {
                return this.NotFound();
            }

            try
            {
                string userId = this.GetUserId();

                Dislike dislike = await this.dislikeService.CreateDislikeAsync(gymId, userId);

                if (dislike.IsDeleted == false)
                {
                    this.TempData[SuccessMessage] = $"You disliked {gym.Name}!";

                    await this.notificationService.CreateNotificationAsync(
                        $"You disliked gym: {gym.Name}",
                        $"/Gym/Details/?gymId={gymId}",
                        userId);
                }
                else
                {
                    this.TempData[SuccessMessage] = $"You Undisliked {gym.Name}!";

                    await this.notificationService.CreateNotificationAsync(
                        $"You Undisliked gym: {gym.Name}",
                        $"/Gym/Details/?gymId={gymId}",
                        userId);
                }
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("Details", "Gym", new { gymId = gymId });
        }
    }
}

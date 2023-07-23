namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Comments;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class CommentController : BaseController
    {
        private readonly ICommentService commentService;
        private readonly IGymService gymService;
        private readonly INotificationService notificationService;

        public CommentController(
            ICommentService _commentService, 
            IGymService _gymService,
            INotificationService _notificationService)
        {
            this.commentService = _commentService;
            this.gymService = _gymService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> AllForGym(string gymId, int page = 1)
        {
            AllCommentsForGymViewModel allCommentsForGymViewModel = new AllCommentsForGymViewModel();

            allCommentsForGymViewModel.GymId = gymId;

            return this.View(allCommentsForGymViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentInputModel createCommentInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.TempData[ErrorMessage] = "The comment is NOT valid!";

                return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
            }

            try
            {
                string userId = this.GetUserId();

                Gym gym = await this.gymService.GetGymByIdAsync(createCommentInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exist!";

                    return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
                }

                await this.commentService.CreateCommentAsync(createCommentInputModel.GymId, userId, createCommentInputModel.Content);

                this.TempData[SuccessMessage] = "You wrote a comment!";

                await this.notificationService.CreateNotificationAsync(
                    $"You commented under: {gym.Name}",
                    $"/Comment/AllForGym?gymId={createCommentInputModel.GymId}&page=1",
                    userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
        }
    }
}

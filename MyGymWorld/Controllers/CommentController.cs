namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Comments;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class CommentController : BaseController
    {
        private const int CommentsPerPage = 5;

        private readonly ICommentService commentService;
        private readonly IGymService gymService;
        private readonly IUserService userService;
        private readonly INotificationService notificationService;

        public CommentController(
            ICommentService _commentService, 
            IGymService _gymService,
            IUserService _userService,
            INotificationService _notificationService)
        {
            this.commentService = _commentService;
            this.gymService = _gymService;
            this.userService = _userService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> AllForGym(string gymId, int page = 1)
        {
            try
            {
                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exist!";

                    return this.RedirectToAction("Index", "Home");
                }

                int count = await this.commentService.GetActiveCommentsCountByGymIdAsync(gymId);

                int totalPages = (int)Math.Ceiling((double)count / CommentsPerPage);
                totalPages = totalPages == 0 ? 1 : totalPages;

                AllCommentsForGymViewModel allCommentsForGymViewModel = new AllCommentsForGymViewModel
                {
                    Comments = await this.commentService.GetActiveCommentsByGymIdAsync(gymId, (page - 1) * CommentsPerPage, CommentsPerPage),
                    CurrentPage = page,
                    PagesCount = totalPages,
                    GymId = gymId,
                    Name = gym.Name
                };

                return this.View(allCommentsForGymViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentInputModel createCommentInputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    this.TempData[ErrorMessage] = "The comment is NOT valid!";

                    return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
                }

                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(createCommentInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exist!";

                    return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
                }

                if (createCommentInputModel.ParentId != null)
                {
                    bool isInTheSameGym = await this.commentService.IsInSameGymByIdAsync(createCommentInputModel.ParentId, createCommentInputModel.GymId);

                    if (isInTheSameGym == false)
                    {
                        this.TempData[ErrorMessage] = "You tried creating a comment on different gym!";

                        return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
                    }

                    Comment? parent = await this.commentService.GetComentByIdAsync(createCommentInputModel.ParentId);

                    if (parent == null)
                    {
                        this.TempData[ErrorMessage] = "No such parent comment!";

                        return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
                    }

                    if (parent != null && parent.ParentId != null)
                    {
                        this.TempData[ErrorMessage] = "You tried creating a comment on reply comment!";

                        return this.RedirectToAction(nameof(AllForGym), new { gymId = createCommentInputModel.GymId });
                    }
                }

                await this.commentService.CreateCommentAsync(createCommentInputModel.GymId, userId, createCommentInputModel.Content, createCommentInputModel.ParentId);

                this.TempData[SuccessMessage] = "You wrote a comment!";

                await this.notificationService.CreateNotificationAsync(
                    $"You commented under {gym.Name}",
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

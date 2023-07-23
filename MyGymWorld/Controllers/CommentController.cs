namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Comments;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class CommentController : BaseController
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService _commentService)
        {
            this.commentService = _commentService;
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

                return this.RedirectToAction(nameof(AllForGym));
            }

            return this.RedirectToAction(nameof(AllForGym));
        }
    }
}

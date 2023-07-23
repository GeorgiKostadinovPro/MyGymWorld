namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Comments;

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
            throw new NotImplementedException();
        }
    }
}

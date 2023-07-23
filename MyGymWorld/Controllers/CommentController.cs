namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;

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
            return this.View();
        }
    }
}

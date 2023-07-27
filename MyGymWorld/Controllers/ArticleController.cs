namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
	using MyGymWorld.Core.Contracts;
	using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Articles;
    using MyGymWorld.Web.ViewModels.Events;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class ArticleController : BaseController
    {
        private readonly IArticleService articleService;
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;
        private readonly IGymService gymService;

        private readonly INotificationService notificationService;

        public ArticleController(
            IArticleService _articleService,
            ICategoryService _categoryService,
            IUserService _userService, 
            IGymService _gymService, 
            INotificationService _notificationService)
        {
            this.articleService = _articleService;
            this.categoryService = _categoryService;
            this.userService = _userService;
            this.gymService = _gymService;

            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> AllForGym([FromQuery] AllArticlesForGymQueryModel queryModel)
        {
            string userId = this.GetUserId();

            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            if (user.ManagerId == null
                || (user.ManagerId != null && !(await this.gymService.CheckIfGymIsManagedByManagerAsync(queryModel.GymId, user.ManagerId.ToString()!))))
            {
                bool hasUserJoinedGym = await this.gymService.CheckIfGymIsJoinedByUserAsync(queryModel.GymId, userId);

                if (hasUserJoinedGym == false)
                {
                    this.TempData[ErrorMessage] = "You have to JOIN the gym to see events!";

                    return this.RedirectToAction("Details", "Gym", new { gymId = queryModel.GymId });
                }
            }

            AllArticlesForGymFilteredAndPagedViewModel allArticlesForGymFilteredAndPagedViewModel = new AllArticlesForGymFilteredAndPagedViewModel()
            {
                TotalArticlesCount = await this.articleService.GetAllActiveArticlesCountByGymIdAsync(queryModel.GymId),
                Articles = await this.articleService.GetAllActiveArticlesFilteredAndPagedByGymIdAsync(queryModel)
            };

            queryModel.Categories = await this.categoryService.GetActiveCategoriesAsync();
            queryModel.TotalArticlesCount = allArticlesForGymFilteredAndPagedViewModel.TotalArticlesCount;
            queryModel.Articles = allArticlesForGymFilteredAndPagedViewModel.Articles;

            return this.View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string articleId)
        {
			bool doesArticleExist = await this.articleService.CheckIfArticleExistsByIdAsync(articleId);

			if (doesArticleExist == false)
			{
				this.TempData[ErrorMessage] = "Such article does NOT exist!";

				return this.NotFound();
			}

			ArticleDetailsViewModel articleDetailsViewModel = await this.articleService.GetArticleDetailsByIdAsync(articleId);

			return this.View(articleDetailsViewModel);
		}
    }
}
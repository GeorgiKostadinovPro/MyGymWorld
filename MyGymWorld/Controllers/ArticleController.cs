namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
	using MyGymWorld.Core.Contracts;
	using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Articles;

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
            Gym gym = await this.gymService.GetGymByIdAsync(queryModel.GymId);

            if (gym == null)
            {
                this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                return this.RedirectToAction("Index", "Home");
            }

            string userId = this.GetUserId();

            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            if (user.ManagerId == null
                || (user.ManagerId != null && !(await this.gymService.CheckIfGymIsManagedByManagerAsync(queryModel.GymId, user.ManagerId.ToString()!))))
            {
                bool hasUserJoinedGym = await this.gymService.CheckIfGymIsJoinedByUserAsync(queryModel.GymId, userId);

                if (hasUserJoinedGym == false)
                {
                    this.TempData[ErrorMessage] = "You have to JOIN the gym to see articles!";

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

        [HttpGet]
        public async Task<IActionResult> Subscribe(string userId, string gymId)
        {
            try
            {
                Gym gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }
                
                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exists!";

                    return this.RedirectToAction(nameof(AllForGym), new { GymId = gymId });
                }

                if (user.ManagerId == null
                    || (user.ManagerId != null && !(await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!))))
                {
                    bool hasUserJoinedGym = await this.gymService.CheckIfGymIsJoinedByUserAsync(gymId, userId);

                    if (hasUserJoinedGym == false)
                    {
                        this.TempData[ErrorMessage] = "You have to JOIN the gym to see articles!";

                        return this.RedirectToAction("Details", "Gym", new { gymId = gymId });
                    }
                }

                bool isUserAlreadySubscribed = await this.articleService.CheckIfUserIsSubscribedForGymArticles(userId, gymId);

                if (isUserAlreadySubscribed)
                {
                    this.TempData[ErrorMessage] = "You have alreeady subscribed!";

                    return this.RedirectToAction(nameof(AllForGym), new { GymId = gymId });
                }

                await this.articleService.SubscribeUserToGymArticlesAsync(userId, gymId);

                this.TempData[SuccessMessage] = "You successfully subscribed!";

                await this.notificationService.CreateNotificationAsync(
                    $"You subscribed for articles by gym - {gym.Name} and now will receive new articles in your Inbox or by email!",
                    $"/Article/AllForGym?gymId={gymId}",
                    user.Id.ToString());
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction(nameof(AllForGym), new { GymId = gymId });
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe(string userId, string gymId)
        {
            try
            {
                Gym gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exists!";

                    return this.RedirectToAction(nameof(AllForGym), new { GymId = gymId });
                }

                if (user.ManagerId == null
                    || (user.ManagerId != null && !(await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!))))
                {
                    bool hasUserJoinedGym = await this.gymService.CheckIfGymIsJoinedByUserAsync(gymId, userId);

                    if (hasUserJoinedGym == false)
                    {
                        this.TempData[ErrorMessage] = "You have to JOIN the gym to see articles!";

                        return this.RedirectToAction("Details", "Gym", new { gymId = gymId });
                    }
                }

                bool isSubscribed = await this.articleService.CheckIfUserIsSubscribedForGymArticles(userId, gymId);

                if (isSubscribed == false)
                {
                    this.TempData[ErrorMessage] = "You are NOT subscribe!";

                    return this.RedirectToAction(nameof(AllForGym), new { GymId = gymId });
                }

                await this.articleService.UnsubscribeUserToGymArticlesAsync(userId, gymId);

                this.TempData[SuccessMessage] = "You successfully Unsubscribed!";

                await this.notificationService.CreateNotificationAsync(
                    $"You Unsubscribed for articles by gym - {gym.Name} and now will NOT receive new articles in your Inbox or by email!",
                    $"/Article/AllForGym?gymId={gymId}",
                    user.Id.ToString());
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction(nameof(AllForGym), new { GymId = gymId });
        }
    }
}
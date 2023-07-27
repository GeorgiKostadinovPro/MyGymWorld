namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Articles;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class ArticleController : ManagerController
    {
        private readonly IArticleService articleService;
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;
        private readonly IGymService gymService;
        private readonly INotificationService notificationService;

        public ArticleController (
            IArticleService articleService,
            ICategoryService _categoryService,
            IUserService _userService,
            IGymService _gymService,
            INotificationService _notificationService)
        {
            this.articleService = articleService;
            this.categoryService = _categoryService;

            this.userService = _userService;
            this.gymService = _gymService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string gymId)
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                Gym gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does not exists!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                CreateArticleInputModel createEventInputModel = new CreateArticleInputModel
                {
                    GymId = gymId,
                    CategoriesListItems = await this.categoryService.GetAllAsSelectListItemsAsync()
                };

                return this.View(createEventInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("AllForGym", "Article", new { area = "", GymId = gymId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateArticleInputModel createArticleInputModel)
        {
            createArticleInputModel.CategoriesListItems = await this.categoryService.GetAllAsSelectListItemsAsync();

            if (!this.ModelState.IsValid)
            {
                return this.View(createArticleInputModel);
            }

            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(createArticleInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                Gym gym = await this.gymService.GetGymByIdAsync(createArticleInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                if (createArticleInputModel.CategoryIds.Count() == 0)
                {
                    this.ModelState.AddModelError("CategoryIds", "You must choose at least one catgeory type!");

                    return this.View(createArticleInputModel);
                }

                createArticleInputModel.Content = new HtmlSanitizer().Sanitize(createArticleInputModel.Content);

                Article createdArticle = await this.articleService.CreateArticleAsync(createArticleInputModel);

                this.TempData[SuccessMessage] = "You created an article!";

                await this.notificationService.CreateNotificationAsync(
                    $"You created an article for {gym.Name}",
                    $"/Article/Details?articleId={createdArticle.Id.ToString()}",
                    userId);
                
                return this.RedirectToAction("AllForGym", "Article", new { area = "", gymId = createArticleInputModel.GymId });
            } 
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }

        [HttpPost]

        public async Task<IActionResult> Delete(string articleId)
        {
            Article? articleToDelete = await this.articleService.GetArticleByIdAsync(articleId);

            if (articleToDelete == null)
            {
                this.TempData[ErrorMessage] = "Such article does NOT exist!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }

            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Article deletedArticle = await this.articleService.DeleteArticleAsync(articleId);

                this.TempData[SuccessMessage] = "You deleted an article!";

                await this.notificationService.CreateNotificationAsync(
                    $"You deleted an article!",
                    $"/Article/Details?articleId={deletedArticle.Id.ToString()}",
                    userId);

                return this.RedirectToActionPermanent("AllForGym", "Article", new { area = "", GymId = articleToDelete.GymId });
            }
            catch (Exception)
            {
                this.TempData[SuccessMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }
    }
}

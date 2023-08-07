namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Articles;
    using System;
    using static MyGymWorld.Common.NotificationMessagesConstants;

	public class ArticleController : ManagerController
	{
        private readonly IConfiguration configuration;

		private readonly IArticleService articleService;
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;
        private readonly IGymService gymService;

        private readonly INotificationService notificationService;
        private readonly IEmailSenderService emailSenderService;

        public ArticleController (
            IConfiguration  _configuration,
            IArticleService _articleService,
            ICategoryService _categoryService,
            IUserService _userService,
            IGymService _gymService,
            INotificationService _notificationService,
            IEmailSenderService _emailSenderService)
        {
            this.configuration = _configuration;

            this.articleService = _articleService;
            this.categoryService = _categoryService;

            this.userService = _userService;
            this.gymService = _gymService;

            this.notificationService = _notificationService;
            this.emailSenderService = _emailSenderService;
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

                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
                    }
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

                Gym? gym = await this.gymService.GetGymByIdAsync(createArticleInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(createArticleInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
                    }
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
                    $"You created an article for {gym.Name}.",
                    $"/Article/Details?articleId={createdArticle.Id.ToString()}",
                    userId);

                IEnumerable<ApplicationUser> subsribers = await this.articleService.GetAllUsersWhoAreSubscribedForGymArticlesAsync(createArticleInputModel.GymId);

                foreach (ApplicationUser subsriber in subsribers)
                {
                   await this.notificationService.CreateNotificationAsync(
                   $"New article for {gym.Name}.",
                   $"/Article/Details?articleId={createdArticle.Id.ToString()}",
                   subsriber.Id.ToString());

                    await this.emailSenderService.SendEmailAsync(
                        subsriber.Email, $"New Article for {gym.Name}", $"<p>Want to read more, click <a href='{this.configuration["ApplicationUrl"]}/Article/Details?articleId={createdArticle.Id}'>here</a></p>");
                }
                
                return this.RedirectToAction("AllForGym", "Article", new { area = "", gymId = createArticleInputModel.GymId });
            } 
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string articleId, string gymId)
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

                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
                    }
                }

                bool doesEventExists = await this.articleService.CheckIfArticleExistsByIdAsync(articleId);

                if (doesEventExists == false)
                {
                    this.TempData[ErrorMessage] = "Such article does NOT exist!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                EditArticleInputModel editArticleInputModel = await this.articleService.GetArticleForEditByIdAsync(articleId);

                editArticleInputModel.CategoriesListItems = await this.categoryService.GetAllAsSelectListItemsAsync();

                return this.View(editArticleInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }

		[HttpPost]
		public async Task<IActionResult> Edit(string articleId, EditArticleInputModel editArticleInputModel)
		{
			editArticleInputModel.CategoriesListItems = await this.categoryService.GetAllAsSelectListItemsAsync();

			if (!this.ModelState.IsValid)
			{
				return this.View(editArticleInputModel);
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

                Gym? gym = await this.gymService.GetGymByIdAsync(editArticleInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
				{
					bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(editArticleInputModel.GymId, user.ManagerId.ToString()!);

					if (isGymManagedByCorrectManager == false)
					{
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
                    }
				}

                editArticleInputModel.Content = new HtmlSanitizer().Sanitize(editArticleInputModel.Content);

				await this.articleService.EditArticleAsync(articleId, editArticleInputModel);

				this.TempData[SuccessMessage] = "You edited an article!";

				await this.notificationService.CreateNotificationAsync(
					$"You edited an article for {gym.Name}.",
					$"/Event/Details?articleId={articleId}",
					userId);

                return this.RedirectToAction("AllForGym", "Article", new { area = "", gymId = editArticleInputModel.GymId });
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

                await this.articleService.DeleteArticleAsync(articleId);

                this.TempData[SuccessMessage] = "You deleted an article!";

                await this.notificationService.CreateNotificationAsync(
                    $"You deleted an article!",
                    $"/Article/AllForGym?gymId={articleToDelete.GymId}",
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
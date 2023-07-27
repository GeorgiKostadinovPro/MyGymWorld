﻿namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
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

                    return this.RedirectToAction("Index", "Home");
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

                    return this.RedirectToAction("Index", "Home");
                }

                CreateArticleInputModel createEventInputModel = new CreateArticleInputModel
                {
                    GymId = gymId,
                    Categories = await this.categoryService.GetActiveCategoriesAsync()
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
            createArticleInputModel.Categories = await this.categoryService.GetActiveCategoriesAsync();

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

                    return this.RedirectToAction("Index", "Home");
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

                    return this.RedirectToAction("Index", "Home");
                }

                Category? category = await this.categoryService.GetCategoryByIdAsync(createArticleInputModel.CategoryId);

                if (createArticleInputModel.CategoryId == "None" || category == null)
                {
                    this.ModelState.AddModelError("CategoryId", "You must choose a valid catgeory type!");

                    return this.View(createArticleInputModel);
                }

                createArticleInputModel.Content = new HtmlSanitizer().Sanitize(createArticleInputModel.Content);

                Article createdArticle = await this.articleService.CreateArticleAsync(createArticleInputModel);

                this.TempData[SuccessMessage] = "You created an article!";

                await this.notificationService.CreateNotificationAsync(
                    $"You created an articlwe for {gym.Name}",
                    $"/Article/Details?articleId={createdArticle.Id.ToString()}",
                    userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("AllForGym", "Article", new { area = "", gymId = createArticleInputModel.GymId });
        }
    }
}

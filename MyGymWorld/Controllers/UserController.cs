﻿namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Users;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly ICountryService countryService;
        private readonly ITownService townService;

        public UserController(
            IUserService _userService,
            ICountryService _countryService,
            ITownService _townService)
        {
            this.userService = _userService;
            this.countryService = _countryService;
            this.townService = _townService;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            string userId = this.GetUserId();

            try
            {
                UserProfileViewModel userProfileViewModel = await this.userService.GetUserToDisplayByIdAsync(userId);

                return this.View(userProfileViewModel);
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            EditUserInputModel editUserInputModel = await this.userService.GetUserForEditByIdAsync(id);

            editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
            editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

            return this.View(editUserInputModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditUserInputModel editUserInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                editUserInputModel.Id = id;
                editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                return this.View(editUserInputModel);
            }

            if (!string.IsNullOrWhiteSpace(editUserInputModel.Address))
            {
                if (editUserInputModel.CountryId == "None")
                {
                    this.ModelState.AddModelError("CountryId", "Country is required when you have address!");
                   
                    editUserInputModel.Id = id;
                    editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                    editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                    return this.View(editUserInputModel);
                }

                if ( editUserInputModel.TownId == "None")
                {
                    this.ModelState.AddModelError("TownId", "Town is required when you have address!");

                    editUserInputModel.Id = id;
                    editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                    editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                    return this.View(editUserInputModel);
                }

                bool isPresent = await this.townService.CheckIfTownIsPresentByCountryIdAsync(editUserInputModel.TownId!, editUserInputModel.CountryId!);

                if (isPresent == false)
                {
                    this.ModelState.AddModelError("TownId", "The town should be in the chosen country!");

                    editUserInputModel.Id = id;
                    editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                    editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                    return this.View(editUserInputModel);
                }
            }

            await this.userService.EditUserAsync(editUserInputModel.Id, editUserInputModel);

            return this.RedirectToAction("UserProfile", "User");
        }
    }
}
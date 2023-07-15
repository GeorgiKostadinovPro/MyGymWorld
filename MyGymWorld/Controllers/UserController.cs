﻿namespace MyGymWorld.Web.Controllers
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Web.ViewModels.Users;
    
    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class UserController : BaseController
    {
        private readonly ICloudinaryService cloudinaryService;

        private readonly IUserService userService;
        private readonly ICountryService countryService;
        private readonly ITownService townService;

        public UserController(
            ICloudinaryService _cloudinaryService,
            IUserService _userService,
            ICountryService _countryService,
            ITownService _townService)
        {
            this.cloudinaryService = _cloudinaryService;

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

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null)
            {
                this.TempData[ErrorMessage] = "Please choose a picture before submitting!!";

                return this.RedirectToAction(nameof(UserProfile));
            }

            if (!this.cloudinaryService.IsFileValid(profilePicture))
            {
                this.TempData[ErrorMessage] = "The allowed types of pictures are jpg, jpeg and png!";

                return this.RedirectToAction(nameof(UserProfile));
            }

            string userId = this.GetUserId();

            (string profilePictureUri, string publicId) = await this.userService.GetUserProfilePictureUriAndPublicIdAsync(userId);

            try
            {
                if (publicId != null)
                {
                    await this.cloudinaryService.DeletePhotoAsync(publicId);
                    await this.userService.DeleteUserProfilePictureAsync(userId);
                }

                ImageUploadResult imageUploadResult = await this.cloudinaryService.UploadPhotoAsync(profilePicture, "MyGymWorld/assets/user-profile-pictures");

                await this.userService.SetUserProfilePictureAsync(userId, imageUploadResult);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when uploading picture!";

                return this.RedirectToAction(nameof(UserProfile));
            }

            return this.RedirectToAction(nameof(UserProfile));
        }

        public async Task<IActionResult> DeleteProfilePicture()
        {
            string userId = this.GetUserId();

            (string profilePictureUri, string publicId) = await this.userService.GetUserProfilePictureUriAndPublicIdAsync(userId);

            if (profilePictureUri == null || publicId == null)
            {
                this.TempData[ErrorMessage] = "You don't have profile picture to delete!";

                return this.RedirectToAction(nameof(UserProfile));
            }

            try
            {
                await this.cloudinaryService.DeletePhotoAsync(publicId);
                await this.userService.DeleteUserProfilePictureAsync(userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when deleting picture!";

                return this.RedirectToAction(nameof(UserProfile));
            }

            return this.RedirectToAction(nameof(UserProfile));
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
            editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
            editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

            if (!this.ModelState.IsValid)
            {
                return this.View(editUserInputModel);
            }

            if (editUserInputModel.FirstName != null && editUserInputModel.LastName == null)
            {
                this.ModelState.AddModelError("LastName", "LastName is required when you have FirstName!");

                return this.View(editUserInputModel);
            }

            if (editUserInputModel.FirstName == null && editUserInputModel.LastName != null)
            {
                this.ModelState.AddModelError("FirstName", "FirstName is required when you have LastName!");

                return this.View(editUserInputModel);
            }

            bool userExistsByPhoneNumber = await this.userService.CheckIfUserExistsByPhoneNumberAsync(editUserInputModel.PhoneNumber);

            if (userExistsByPhoneNumber)
            {
                this.ModelState.AddModelError("PhoneNumber", "User with this phone exists!");

                return this.View(editUserInputModel);
            }

            if (!string.IsNullOrWhiteSpace(editUserInputModel.Address))
            {
                if (editUserInputModel.CountryId == "None")
                {
                    this.ModelState.AddModelError("CountryId", "Country is required when you have address!");
                   
                    return this.View(editUserInputModel);
                }

                if (editUserInputModel.TownId == "None")
                {
                    this.ModelState.AddModelError("TownId", "Town is required when you have address!");

                    return this.View(editUserInputModel);
                }

                bool isPresent = await this.townService.CheckIfTownIsPresentByCountryIdAsync(editUserInputModel.TownId!, editUserInputModel.CountryId!);

                if (isPresent == false)
                {
                    this.ModelState.AddModelError("TownId", "The town should be in the chosen country!");

                    return this.View(editUserInputModel);
                }
            }

            if (string.IsNullOrWhiteSpace(editUserInputModel.Address))
            {
                if (editUserInputModel.CountryId != "None"
                    && editUserInputModel.TownId != null)
                {
                    this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");
                    this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");
                    
                    return this.View(editUserInputModel);
                }
                else if (editUserInputModel.CountryId != "None")
                {
                    this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");

                    return this.View(editUserInputModel);
                }
                else if (editUserInputModel.TownId != null 
                    && editUserInputModel.TownId != "None")
                {
                    this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");

                    return this.View(editUserInputModel);
                }
            }

            bool isUsernamePresent = await this.userService.CheckIfUserExistsByUsernameAsync(editUserInputModel.UserName);
            var user = await this.userService.GetUserByIdAsync(id);

            if (isUsernamePresent && user.Id != Guid.Parse(id))
            {
                this.ModelState.AddModelError("UserName", "This username is already taken!");

                return this.View(editUserInputModel);
            }

            try
            {
                await this.userService.EditUserAsync(editUserInputModel.Id, editUserInputModel);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.View(editUserInputModel);
            }

            return this.RedirectToAction("UserProfile", "User");
        }
    }
}
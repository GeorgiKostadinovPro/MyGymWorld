namespace MyGymWorld.Web.Controllers
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
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
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

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
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

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

                (string profilePictureUri, string publicId) = await this.userService.GetUserProfilePictureUriAndPublicIdAsync(userId);

                if (publicId != null)
                {
                    await this.cloudinaryService.DeletePhotoAsync(publicId);
                    await this.userService.DeleteUserProfilePictureAsync(userId);
                }

                ImageUploadResult imageUploadResult = await this.cloudinaryService.UploadPhotoAsync(profilePicture, "MyGymWorld/assets/user-profile-pictures");

                await this.userService.SetUserProfilePictureAsync(userId, imageUploadResult);

                this.TempData[SuccessMessage] = "You have successfully uploaded a picture!";
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when uploading picture!";
            }

            return this.RedirectToAction(nameof(UserProfile));
        }

        public async Task<IActionResult> DeleteProfilePicture()
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                (string profilePictureUri, string publicId) = await this.userService.GetUserProfilePictureUriAndPublicIdAsync(userId);

                if (profilePictureUri == null || publicId == null)
                {
                    this.TempData[ErrorMessage] = "You don't have profile picture to delete!";

                    return this.RedirectToAction(nameof(UserProfile));
                }

                await this.cloudinaryService.DeletePhotoAsync(publicId);

                await this.userService.DeleteUserProfilePictureAsync(userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when deleting picture!";
            }

            return this.RedirectToAction(nameof(UserProfile));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                ApplicationUser user = await this.userService.GetUserByIdAsync(id);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                EditUserInputModel editUserInputModel = await this.userService.GetUserForEditByIdAsync(id);

                editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                return this.View(editUserInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction(nameof(UserProfile));
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditUserInputModel editUserInputModel)
        {
            try
            {
                ApplicationUser user = await this.userService.GetUserByIdAsync(id);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

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

                if (editUserInputModel.PhoneNumber != null)
                {
                    bool userExistsByPhoneNumber = await this.userService.CheckIfUserExistsByPhoneNumberAsync(editUserInputModel.PhoneNumber);

                    if (userExistsByPhoneNumber)
                    {
                        this.ModelState.AddModelError("PhoneNumber", "User with this phone exists!");

                        return this.View(editUserInputModel);
                    }
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

                if (isUsernamePresent && user.Id.ToString() != id)
                {
                    this.ModelState.AddModelError("UserName", "This username is already taken!");

                    return this.View(editUserInputModel);
                }

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
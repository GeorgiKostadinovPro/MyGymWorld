namespace MyGymWorld.Web.Controllers
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

            if (editUserInputModel.FirstName != null && editUserInputModel.LastName == null)
            {
                this.ModelState.AddModelError("LastName", "LastName is required when you have FirstName!");

                editUserInputModel.Id = id;
                editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                return this.View(editUserInputModel);
            }

            if (editUserInputModel.FirstName == null && editUserInputModel.LastName != null)
            {
                this.ModelState.AddModelError("FirstName", "FirstName is required when you have LastName!");

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

            if (string.IsNullOrWhiteSpace(editUserInputModel.Address))
            {
                if (editUserInputModel.CountryId != "None"
                    && editUserInputModel.TownId != null)
                {
                    this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");
                    this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");
                    
                    editUserInputModel.Id = id;
                    editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                    editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                    return this.View(editUserInputModel);
                }
                else if (editUserInputModel.CountryId != "None")
                {
                    this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");

                    editUserInputModel.Id = id;
                    editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                    editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                    return this.View(editUserInputModel);
                }
                else if (editUserInputModel.TownId != null 
                    && editUserInputModel.TownId != "None")
                {
                    this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");

                    editUserInputModel.Id = id;
                    editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                    editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                    return this.View(editUserInputModel);
                }
            }

            bool isUsernamePresent = await this.userService.CheckIfUserExistsByUsernameAsync(editUserInputModel.UserName);
            var user = await this.userService.GetUserByIdAsync(id);

            if (isUsernamePresent && user.Id != Guid.Parse(id))
            {
                this.ModelState.AddModelError("UserName", "This username is already taken!");

                editUserInputModel.Id = id;
                editUserInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                editUserInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                return this.View(editUserInputModel);
            }

            try
            {
                await this.userService.EditUserAsync(editUserInputModel.Id, editUserInputModel);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("ProfilePicture", ex.Message);

                return this.View(editUserInputModel);
            }

            return this.RedirectToAction("UserProfile", "User");
        }
    }
}
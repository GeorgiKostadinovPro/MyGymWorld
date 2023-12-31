﻿namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using CloudinaryDotNet.Actions;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;

    using static MyGymWorld.Common.ExceptionConstants;
    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class GymController : ManagerController
    {
        private const int GymsPerPage = 2;

        private readonly ICloudinaryService cloudinaryService; 
        private readonly INotificationService notificationService;

        private readonly IManagerService managerService;
        private readonly IUserService userService;
        private readonly IGymService gymService;
        private readonly ICountryService countryService;
        private readonly ITownService townService;

        public GymController(
            ICloudinaryService _cloudinaryService,
            INotificationService _notificationService,
            IManagerService _managerService,
            IUserService _userService,
            IGymService _gymService,
            ICountryService _countryService,
            ITownService _townService)
        {
            this.cloudinaryService = _cloudinaryService;
            this.notificationService = _notificationService;

            this.managerService = _managerService;
            this.userService = _userService;
            this.gymService = _gymService;
            this.countryService = _countryService;
            this.townService = _townService;
        }

        [HttpGet]
        public async Task<IActionResult> Active(int page = 1)
        {
            try
            {
                ApplicationUser user = await this.userService.GetUserByIdAsync(this.GetUserId());

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (!User.IsInRole("Manager"))
                {
                    this.TempData[ErrorMessage] = "You do NOT have rights to open this page!";

                    return this.RedirectToAction("Index", "Home");
                }

                int count = await this.gymService.GetActiveOrDeletedGymsCountForManagementAsync(user.ManagerId.Value.ToString(), false);

                int totalPages = (int)Math.Ceiling((double)count / GymsPerPage);

                AllGymsForManagementViewModel allGymsForManagement = new AllGymsForManagementViewModel
                {
                    Gyms = await this.gymService
                    .GetActiveOrDeletedForManagementAsync(user.ManagerId.Value.ToString(), false, (page - 1) * GymsPerPage, GymsPerPage),
                    CurrentPage = page,
                    PagesCount = totalPages
                };

                return this.View(allGymsForManagement);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }

		[HttpGet]
		public async Task<IActionResult> Deleted(int page = 1)
		{
            try
            {
                ApplicationUser user = await this.userService.GetUserByIdAsync(this.GetUserId());

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (!User.IsInRole("Manager"))
                {
                    this.TempData[ErrorMessage] = "You do NOT have rights to open this page!";

                    return this.RedirectToAction("Index", "Home");
                }

                int count = await this.gymService.GetActiveOrDeletedGymsCountForManagementAsync(user.ManagerId.Value.ToString(), true);

                int totalPages = (int)Math.Ceiling((double)count / GymsPerPage);

                AllGymsForManagementViewModel allGymsForManagement = new AllGymsForManagementViewModel
                {
                    Gyms = await this.gymService
                    .GetActiveOrDeletedForManagementAsync(user.ManagerId.Value.ToString(), true, (page - 1) * GymsPerPage, GymsPerPage),
                    CurrentPage = page,
                    PagesCount = totalPages
                };

                return this.View(allGymsForManagement);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
			
		}

		[HttpGet]
        public async Task<IActionResult> Create()
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

                Manager? manager = await this.managerService.GetManagerByUserIdAsync(this.GetUserId());

                int count = await this.gymService.GetActiveOrDeletedGymsCountForManagementAsync(user.ManagerId.Value.ToString(), false);

                if ((int)manager!.ManagerType == 0 && count > 0)
                {
                    this.TempData[ErrorMessage] = "You are NOT allowed to create more than one gym!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                CreateGymInputModel createGymInputModel = new CreateGymInputModel
                {
                    GymTypes = this.gymService.GetAllGymTypes(),
                    CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync(),
                    TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync()
                };

                return this.View(createGymInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGymInputModel createGymInputModel)
        {
            try
            {
                createGymInputModel.GymTypes = this.gymService.GetAllGymTypes();
                createGymInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                createGymInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                if (!this.ModelState.IsValid)
                {
                    return this.View(createGymInputModel);
                }

                if (!this.cloudinaryService.IsFileValid(createGymInputModel.LogoFile))
                {
                    this.ModelState.AddModelError("LogoFile", "The logo is required and the allowed types of pictures are jpg, jpeg and png!");

                    return this.View(createGymInputModel);
                }

                if (createGymInputModel.GalleryImagesFiles == null)
                {
                    this.ModelState.AddModelError("GalleryImagesFiles", "You must upload at least one gym picture!");

                    return this.View(createGymInputModel);
                }

                foreach (var picture in createGymInputModel.GalleryImagesFiles)
                {
                    if (!this.cloudinaryService.IsFileValid(picture))
                    {
                        this.ModelState.AddModelError("GalleryImagesFiles", "The allowed types of pictures are jpg, jpeg and png!");

                        return this.View(createGymInputModel);
                    }
                }

                if (createGymInputModel.GymType == "None")
                {
                    this.ModelState.AddModelError(createGymInputModel.GymType, "You have to choose a Gym Type!");

                    return this.View(createGymInputModel);
                }

                if (!string.IsNullOrWhiteSpace(createGymInputModel.Address))
                {
                    if (createGymInputModel.CountryId == "None")
                    {
                        this.ModelState.AddModelError("CountryId", "Country is required when you have address!");

                        return this.View(createGymInputModel);
                    }

                    if (createGymInputModel.TownId == "None")
                    {
                        this.ModelState.AddModelError("TownId", "Town is required when you have address!");

                        return this.View(createGymInputModel);
                    }

                    bool isPresent = await this.townService.CheckIfTownIsPresentByCountryIdAsync(createGymInputModel.TownId!, createGymInputModel.CountryId!);

                    if (isPresent == false)
                    {
                        this.ModelState.AddModelError("TownId", "The town should be in the chosen country!");

                        return this.View(createGymInputModel);
                    }
                }

                if (string.IsNullOrWhiteSpace(createGymInputModel.Address))
                {
                    if (createGymInputModel.CountryId != "None"
                        && createGymInputModel.TownId != null)
                    {
                        this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");
                        this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");

                        return this.View(createGymInputModel);
                    }
                    else if (createGymInputModel.CountryId != "None")
                    {
                        this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");

                        return this.View(createGymInputModel);
                    }
                    else if (createGymInputModel.TownId != null
                        && createGymInputModel.TownId != "None")
                    {
                        this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");
                        return this.View(createGymInputModel);
                    }
                }

                createGymInputModel.Description = new HtmlSanitizer().Sanitize(createGymInputModel.Description);

                bool isGymTypeValid = Enum.TryParse<GymType>(createGymInputModel.GymType, true, out GymType gymType);

                if (!isGymTypeValid)
                {
                    throw new InvalidOperationException(ExceptionConstants.GymErrors.InvalidGymType);
                }

                ImageUploadResult logoResultParams = await this.cloudinaryService.UploadPhotoAsync(createGymInputModel.LogoFile, "MyGymWorld/assets/gyms-logo-pictures");
               
                createGymInputModel.LogoResultParams = logoResultParams;

                foreach (var imageFile in createGymInputModel.GalleryImagesFiles)
                {
                    ImageUploadResult imageResultParams = await this.cloudinaryService.UploadPhotoAsync(imageFile, "MyGymWorld/assets/gyms-gallery-pictures");

                    createGymInputModel.GalleryImagesResultParams.Add(imageResultParams);
                }
               
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                await this.gymService.CreateGymAsync(user.ManagerId!.Value.ToString(), createGymInputModel);

                this.TempData[SuccessMessage] = $"You successfully created a gym!";

                await this.notificationService.CreateNotificationAsync(
                   $"You created a gym.",
                   "/Manager/Gym/Active?page=1",
                   this.GetUserId());
                
                return this.RedirectToAction(nameof(Active));
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.View(createGymInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when trying to add the gym!";

                return this.View(createGymInputModel);
            }
        }

		[HttpGet]
		public async Task<IActionResult> Edit(string gymId)
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

                bool doesGymExists = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

                if (!doesGymExists)
                {
                    this.TempData[ErrorMessage] = GymErrors.InvalidGymId;

                    return this.RedirectToAction("Index", "Home");
                }

                EditGymInputModel editGymInputModel = await this.gymService.GetGymForEditByIdAsync(gymId);

                editGymInputModel.GymTypes = this.gymService.GetAllGymTypes();
                editGymInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                editGymInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                return this.View(editGymInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
			
		}

        [HttpPost]
        public async Task<IActionResult> Edit([FromQuery]string gymId, EditGymInputModel editGymInputModel)
        {
            try
            {
                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                editGymInputModel.Id = gymId;
                editGymInputModel.GymTypes = this.gymService.GetAllGymTypes();
                editGymInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
                editGymInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

                if (!this.ModelState.IsValid)
                {
                    return this.View(editGymInputModel);
                }

                bool hasLogo = false;
                bool hasGalleryImages = false;

                if (editGymInputModel.LogoFile != null)
                {
                    hasLogo = true;
                }

                if (editGymInputModel.GalleryImagesFiles != null)
                {
                    hasGalleryImages = true;
                }

                if (hasLogo == true && !this.cloudinaryService.IsFileValid(editGymInputModel.LogoFile))
                {
                    this.ModelState.AddModelError("LogoFile", "The logo is required and the allowed types of pictures are jpg, jpeg and png!");

                    return this.View(editGymInputModel);
                }

                if (hasGalleryImages == true)
                {
                    foreach (var picture in editGymInputModel.GalleryImagesFiles)
                    {
                        if (!this.cloudinaryService.IsFileValid(picture))
                        {
                            this.ModelState.AddModelError("GalleryImagesFiles", "The allowed types of pictures are jpg, jpeg and png!");

                            return this.View(editGymInputModel);
                        }
                    }
                }

                if (editGymInputModel.GymType == "None")
                {
                    this.ModelState.AddModelError(editGymInputModel.GymType, "You have to choose a Gym Type!");

                    return this.View(editGymInputModel);
                }

                if (!string.IsNullOrWhiteSpace(editGymInputModel.Address))
                {
                    if (editGymInputModel.CountryId == "None")
                    {
                        this.ModelState.AddModelError("CountryId", "Country is required when you have address!");

                        return this.View(editGymInputModel);
                    }

                    if (editGymInputModel.TownId == "None")
                    {
                        this.ModelState.AddModelError("TownId", "Town is required when you have address!");

                        return this.View(editGymInputModel);
                    }

                    bool isPresent = await this.townService.CheckIfTownIsPresentByCountryIdAsync(editGymInputModel.TownId!, editGymInputModel.CountryId!);

                    if (isPresent == false)
                    {
                        this.ModelState.AddModelError("TownId", "The town should be in the chosen country!");

                        return this.View(editGymInputModel);
                    }
                }

                if (string.IsNullOrWhiteSpace(editGymInputModel.Address))
                {
                    if (editGymInputModel.CountryId != "None"
                        && editGymInputModel.TownId != null)
                    {
                        this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");
                        this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");

                        return this.View(editGymInputModel);
                    }
                    else if (editGymInputModel.CountryId != "None")
                    {
                        this.ModelState.AddModelError("CountryId", "You cannot choose a country without an address!");

                        return this.View(editGymInputModel);
                    }
                    else if (editGymInputModel.TownId != null
                        && editGymInputModel.TownId != "None")
                    {
                        this.ModelState.AddModelError("TownId", "You cannot choose a town without an address!");
                        return this.View(editGymInputModel);
                    }
                }

                editGymInputModel.Description = new HtmlSanitizer().Sanitize(editGymInputModel.Description);

                bool isGymTypeValid = Enum.TryParse<GymType>(editGymInputModel.GymType, true, out GymType gymType);

                if (!isGymTypeValid)
                {
                    throw new InvalidOperationException(ExceptionConstants.GymErrors.InvalidGymType);
                }

                if (hasLogo == true)
                {
                    ImageUploadResult logoResultParams = await this.cloudinaryService.UploadPhotoAsync(editGymInputModel.LogoFile, "MyGymWorld/assets/gyms-logo-pictures"); 
                    
                    editGymInputModel.LogoResultParams = logoResultParams;
                }
                    
                if (hasGalleryImages == true)
                {
                    foreach (var imageFile in editGymInputModel.GalleryImagesFiles)
                    {
                        ImageUploadResult imageResultParams = await this.cloudinaryService.UploadPhotoAsync(imageFile, "MyGymWorld/assets/gyms-gallery-pictures");

                        editGymInputModel.GalleryImagesResultParams.Add(imageResultParams);
                    }
                }

                await this.gymService.EditGymAsync(gymId, editGymInputModel);

                this.TempData[SuccessMessage] = $"You successfully edited {gym.Name}!";

                await this.notificationService.CreateNotificationAsync(
                   $"You edited {gym.Name}.",
                   "/Manager/Gym/Active?page=1",
                   this.GetUserId());
                
                return this.RedirectToAction(nameof(Active));
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.View(editGymInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when trying to add the gym!";

                return this.View(editGymInputModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string gymId)
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

                Gym? gymToDelete = await this.gymService.GetGymByIdAsync(gymId);

                if (gymToDelete == null)
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

                await this.gymService.DeleteGymAsync(gymId);

                this.TempData[SuccessMessage] = $"You successfully deleted {gymToDelete.Name}!";

                await this.notificationService.CreateNotificationAsync(
                    $"You deleted {gymToDelete.Name}.",
                    "/Manager/Gym/Deleted?page=1",
                    userId);

                return this.RedirectToAction(nameof(Deleted));
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction(nameof(Active));
            }
        }
    }
}

namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class GymController : ManagerController
    {
        private readonly ICloudinaryService cloudinaryService;

        private readonly IGymService gymService;
        private readonly ICountryService countryService;
        private readonly ITownService townService;

        public GymController(
            ICloudinaryService _cloudinaryService,
            IGymService _gymService,
            ICountryService _countryService,
            ITownService _townService)
        {
            this.cloudinaryService = _cloudinaryService;

            this.gymService = _gymService;
            this.countryService = _countryService;
            this.townService = _townService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateGymInputModel createGymInputModel = new CreateGymInputModel
            {
                GymTypes = this.gymService.GetAllGymTypes(),
                CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync(),
                TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync()
            };

            return this.View(createGymInputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGymInputModel createGymInputModel)
        {
            createGymInputModel.GymTypes = this.gymService.GetAllGymTypes();
            createGymInputModel.CountriesSelectList = await this.countryService.GetAllAsSelectListItemsAsync();
            createGymInputModel.TownsSelectList = await this.townService.GetAllAsSelectListItemsAsync();

            if (!this.ModelState.IsValid)
            {
                return this.View(createGymInputModel);
            }

            if (!this.cloudinaryService.IsFileValid(createGymInputModel.LogoUrl))
            {
                this.ModelState.AddModelError("LogoUrl", "The logo is required and the allowed types of pictures are jpg, jpeg and png!");

                return this.View(createGymInputModel);
            }

            if (createGymInputModel.GalleryImagesUri == null)
            {
                this.ModelState.AddModelError("GalleryImagesUri", "You must upload at least one gym picture!");

                return this.View(createGymInputModel);
            }

            foreach (var picture in createGymInputModel.GalleryImagesUri)
            {
                if (!this.cloudinaryService.IsFileValid(picture))
                {
                    this.ModelState.AddModelError("GalleryImagesUri", "The allowed types of pictures are jpg, jpeg and png!");

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

            try
            {
                await this.gymService.CreateGymAsync(createGymInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong when trying to add the gym!";

                return this.View(createGymInputModel);
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Active()
        {
            return this.View();
        }
    }
}

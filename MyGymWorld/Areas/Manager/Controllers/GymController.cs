namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;

    public class GymController : ManagerController
    {
        private readonly IGymService gymService;
        private readonly ICountryService countryService;
        private readonly ITownService townService;

        public GymController(
            IGymService _gymService,
            ICountryService _countryService,
            ITownService _townService)
        {
            this.gymService = _gymService;
            this.countryService = _countryService;
            this.townService = _townService;
        }

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
    }
}

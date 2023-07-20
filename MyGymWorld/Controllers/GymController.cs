namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Gyms;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class GymController : BaseController
    {
        private readonly IGymService gymService;

        public GymController(IGymService _gymService)
        {
            this.gymService = _gymService;
        }

        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllGymsQueryModel queryModel)
        {
            AllGymsFilteredAndPagedViewModel allGymsFilteredAndPagedViewModel = new AllGymsFilteredAndPagedViewModel
            {
                TotalGymsCount = await this.gymService.GetActiveGymsCountAsync(),
                Gyms = await this.gymService.GetAllFilteredAndPagedActiveGymsAsync(queryModel)
            };
               
            queryModel.GymTypes = this.gymService.GetAllGymTypes();
            queryModel.TotalGymsCount = allGymsFilteredAndPagedViewModel.TotalGymsCount;
            queryModel.Gyms = allGymsFilteredAndPagedViewModel.Gyms;

            return this.View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string gymId)
        {
            bool doesGymExist = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

            if (!doesGymExist)
            {
                this.TempData[ErrorMessage] = "Such Gym does NOT exists!";

                return this.NotFound();
            }

            GymDetailsViewModel gymDetailsViewModel = await this.gymService.GetGymDetailsByIdAsync(gymId);

            return this.View(gymDetailsViewModel);
        }
    }
}
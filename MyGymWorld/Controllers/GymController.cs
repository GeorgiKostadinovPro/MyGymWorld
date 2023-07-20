namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Gyms;

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
    }
}
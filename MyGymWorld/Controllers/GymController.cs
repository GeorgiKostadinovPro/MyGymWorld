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
        public async Task<IActionResult> All()
        {
            AllGymForDisplayViewModel allGymToDisplayViewModel = new AllGymForDisplayViewModel
            { 
                Gyms = await this.gymService.GetAllActiveGymsAsync()
            };

            return this.View(allGymToDisplayViewModel);
        }
    }
}

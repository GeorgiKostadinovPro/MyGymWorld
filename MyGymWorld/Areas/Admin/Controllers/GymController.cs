namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.Areas.Administration.Controllers;

    public class GymController : AdminController
    {
        private readonly IGymService gymService;

        public GymController(IGymService _gymService)
        {
            this.gymService = _gymService;
        }

        public async Task<IActionResult> Active(int page = 1)
        {
            return View();
        }
    }
}

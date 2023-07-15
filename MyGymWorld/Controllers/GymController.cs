namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class GymController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> All()
        {
            return View();
        }
    }
}

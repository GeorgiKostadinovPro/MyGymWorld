namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class GymController : ManagerController
    {
        public async Task<IActionResult> Create()
        {
            return this.View();
        }
    }
}

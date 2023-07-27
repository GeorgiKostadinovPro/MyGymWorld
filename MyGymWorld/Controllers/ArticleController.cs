namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ArticleController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> AllForGym(string gymId)
        {
            return this.View();
        }
    }
}

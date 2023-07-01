using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyGymWorld.Common;
using MyGymWorld.Core.Contracts;
using MyGymWorld.Models;
using MyGymWorld.Web.Controllers;
using System.Diagnostics;

namespace MyGymWorld.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        public readonly IUserService userService;

        public HomeController(
            ILogger<HomeController> logger,
            IUserService _userService)
        {
            _logger = logger;

            this.userService = _userService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            string userId = this.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return this.View();
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
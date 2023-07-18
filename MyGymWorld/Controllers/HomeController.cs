﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyGymWorld.Core.Contracts;
using MyGymWorld.Models;
using MyGymWorld.Web.Controllers;
using MyGymWorld.Web.ViewModels.Gyms;
using System.Diagnostics;

namespace MyGymWorld.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IGymService gymService;

        public HomeController(
            ILogger<HomeController> logger,
            IGymService _gymService)
        {
            _logger = logger;

            this.gymService = _gymService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            AllGymForDisplayViewModel allGymToDisplayViewModel = new AllGymForDisplayViewModel
            {
                Gyms = await this.gymService.GetAllActiveGymsAsync()
            };

            return this.View(allGymToDisplayViewModel);

            return this.View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                return this.View("Error404");
            }

            if (statusCode == 401)
            {
                return this.View("Error401");
            }

            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
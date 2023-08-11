namespace MyGymWorld.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Models;
    using MyGymWorld.Web.Controllers;
    using MyGymWorld.Web.ViewModels.Gyms;
    using MyGymWorld.Web.ViewModels.Home;
    using Stripe;
    using System.Diagnostics;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IGymService gymService;
        private readonly IUserService userService;
        private readonly IEmailSenderService emailSenderService;

        public HomeController(
            ILogger<HomeController> logger,
            IGymService _gymService,
            IUserService _userService,
            IEmailSenderService _emailSenderService)
        {
            _logger = logger;

            this.gymService = _gymService;
            this.userService = _userService;
            this.emailSenderService = _emailSenderService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            AllGymForDisplayViewModel allGymToDisplayViewModel = new AllGymForDisplayViewModel
            {
                NewestGyms = await this.gymService.GetTop10NewestActiveGymsAsync(),
                MostLikedGyms = await this.gymService.GetTop10MostLikedActiveGymsAsync() 
            };

            return this.View(allGymToDisplayViewModel);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            ContactInfoInputModel contactInfoInputModel = new ContactInfoInputModel();

            return this.View(contactInfoInputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactInfoInputModel contactInfoInputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(contactInfoInputModel);
                }

                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return this.RedirectToAction(nameof(Error), new { statusCode = 401 });
                }

                if (user.Email != contactInfoInputModel.Email)
                {
                    this.ModelState.AddModelError("Email", "Your email is NOT correct!");

                    return this.View(contactInfoInputModel);
                }

                ApplicationUser admin = await this.userService.GetAdministratorAsync();

                await this.emailSenderService.SendEmailAsync(admin.Email, contactInfoInputModel.Subject, contactInfoInputModel.Content);

                this.TempData[SuccessMessage] = "You successfully sent a question!";
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
                
            }
            
            return this.RedirectToAction(nameof(Index));
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

            if (statusCode == 403)
            {
                return this.View("Error403");
            }

            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
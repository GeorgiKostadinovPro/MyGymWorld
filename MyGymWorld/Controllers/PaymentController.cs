﻿namespace MyGymWorld.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using MyGymWorld.Core.Contracts;
	using MyGymWorld.Data.Models;

	using Stripe.Checkout;

    using static MyGymWorld.Common.NotificationMessagesConstants;

	public class PaymentController : BaseController
	{
		public string? SessionId { get; set; }

		private readonly IConfiguration configuration;

		private readonly IMembershipService membershipService;
		private readonly IGymService gymService;
        private readonly IUserService userService;

        public PaymentController (
			IConfiguration _configuration, 
			IMembershipService _membershipService,
			IGymService _gymService,
            IUserService _userService)
        {
            this.configuration = _configuration;

			this.membershipService = _membershipService;
			this.gymService = _gymService;
            this.userService = _userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(string userId, string membershipId)
        {
            try
            {
                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return this.RedirectToAction("Error", "Home", new { statusCode = 401 });
                }

                string domain = this.configuration["ApplicationUrl"];

                Membership? membership =
                    await this.membershipService.GetMembershipByIdAsync(membershipId);

                if (membership == null)
                {
                    return this.RedirectToAction("Error", "Home", new { statusCode = 404 });
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(membership.GymId.ToString());

                if (gym == null)
                {
                    return this.RedirectToAction("Error", "Home", new { statusCode = 404 });
                }

                if (domain != null)
                {
                    string membershipType
                    = string.Concat(membership.MembershipType.ToString(), " Membership");

                    string description
                        = string.Concat(membershipType, " for ", gym.Name);

                    SessionCreateOptions sessionCreateOptions = new SessionCreateOptions
                    {
                        CustomerEmail = user.Email,
                        PaymentMethodTypes = new List<string>
                        {
                            "card"
                        },
                        LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    UnitAmount = Convert.ToInt32(membership.Price) * 100,
                                    Currency = "usd",
                                    ProductData = new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = membershipType,
                                        Description = description,
                                        Images = new List<string> { gym.LogoUri }
                                    }
                                },
                                Quantity = 1
                            }
                        },
                        Mode = "payment",
                        SuccessUrl = string.Concat(domain, $"/Membership/Buy?membershipId={membershipId}"),
                        CancelUrl = string.Concat(domain, $"/Membership/Details?membershipId={membershipId}")
                    };

                    SessionService service = new SessionService();
                    Session session = await service.CreateAsync(sessionCreateOptions);

                    this.SessionId = session.Id;

                    return this.Redirect(session.Url);
                }
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.Redirect($"/Membership/Details?membershipId={membershipId}");
            }
            
            return this.StatusCode(500);
        }
    }
}
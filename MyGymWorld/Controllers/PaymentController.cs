namespace MyGymWorld.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
	using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Memberships;
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

        [HttpGet]
        public async Task<IActionResult> SuccessfulPayment(string userId, string membershipId)
        {
            try
            {
                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exist";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                UserMembership? userMembership = await this.membershipService.GetUserMembershipAsync(userId, membershipId);

                if (userMembership == null)
                {
                    this.TempData[ErrorMessage] = "Such payment does NOT exist";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                PayedMembershipViewModel payedMembershipViewModel = new PayedMembershipViewModel
                {
                    Id = membershipId,
                    UserEmail = user.Email,
                    Price = userMembership.Membership.Price.ToString("C"),
                    MembershipType = userMembership.Membership.MembershipType.ToString(),
                    PurchasedOn = userMembership.CreatedOn
                };

                return this.View(payedMembershipViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyPaymentsForMemberships(int page = 1)
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user doesNOT exist";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                int count = await this.membershipService.GetAllActiveUserMembershipsCountByUserIdAsync(userId);

                int totalPages = (int)Math.Ceiling((double)count / GlobalConstants.MembershipConstants.MembershipsPerPage);
                totalPages = totalPages == 0 ? 1 : totalPages;

                AllUserMembershipPaymentsViewModel allUserMembershipPaymentsViewModel = new AllUserMembershipPaymentsViewModel
                {
                    Memberships = await this.membershipService.GetActivePaymentsByUserIdAsync(userId),
                    CurrentPage = page,
                    PagesCount = totalPages,
                    UserId = userId
                };

                return this.View(allUserMembershipPaymentsViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }
    }
}
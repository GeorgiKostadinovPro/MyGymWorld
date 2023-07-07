namespace MyGymWorld.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Web.Controllers;

    [Authorize(Roles = ApplicationRoleConstants.AdministratorRoleName)]
    [Area("Admin")]
    public class AdminController : BaseController
    {
    }
}

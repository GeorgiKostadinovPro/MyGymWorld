namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Web.Controllers;

    [Authorize(Roles = ApplicationRoleConstants.ManagerRoleName)]
    [Area(GlobalConstants.ManagerAreaName)]
    public class ManagerController : BaseController
    {
    }
}

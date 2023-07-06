namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Managers;
    using System;
    using System.Threading.Tasks;

    public class ManagerService : IManagerService
    {
        public async Task<BecomeManagerInputModel> GetUserToBecomeManagerByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

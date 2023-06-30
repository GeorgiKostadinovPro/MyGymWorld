namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Users;
    using System;
    using System.Threading.Tasks;

    public class AccountService : IAccountService
    {
        public async Task RegisterUserAsync(RegisterUserInputModel registerUserInputModel)
        {
            throw new NotImplementedException();
        }
        
        public async Task AuthenticateAsync(LoginUserInputModel loginUserInputModel)
        {
            throw new NotImplementedException();
        }
    }
}

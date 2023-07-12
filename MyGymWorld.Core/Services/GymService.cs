namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System;
    using System.Threading.Tasks;

    public class GymService : IGymService
    {
        private readonly IRepository repository;

        public GymService(IRepository _repository)
        {
            this.repository = _repository;
        }

        public Task CreateGymAsync(CreateGymInputModel createGymInputModel)
        {
            throw new NotImplementedException();
        }
    }
}

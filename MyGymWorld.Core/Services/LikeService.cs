namespace MyGymWorld.Core.Services
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System.Threading.Tasks;

    public class LikeService : ILikeService
    {
        private readonly IRepository repository;

        private readonly IGymService gymService;
        private readonly IUserService userService;

        public LikeService(
            IRepository _repository,
            IGymService _gymService,
            IUserService _userService)
        {
            this.repository = _repository;

            this.gymService = _gymService;
            this.userService = _userService;
        }

        public async Task<Like> CreateLikeAsync(string gymId, string userId)
        {
            Like? like = await this.repository.All<Like>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (like == null)
            {
                like = new Like 
                {
                    GymId = Guid.Parse(gymId),
                    UserId = Guid.Parse(userId),
                    CreatedOn = DateTime.UtcNow
                };

                await this.repository.AddAsync(like);
            }
            else
            {
                if (like.IsDeleted == true)
                {
                    like.IsDeleted = false;
                    like.DeletedOn = null;
                }
                else
                {
                    like.IsDeleted = true;
                    like.DeletedOn = DateTime.UtcNow;
                }
            }

            return like;
        }
        
        public async Task<bool> CheckIfUserLikedGymAsync(string gymId, string userId)
        {
            Like? like = await this.repository.All<Like>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (like == null)
            {
                return false;
            }

            if (like.IsDeleted == true)
            {
                return false;
            }

            return true;
        }
    }
}

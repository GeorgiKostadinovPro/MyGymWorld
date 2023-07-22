namespace MyGymWorld.Core.Services
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System;
    using System.Threading.Tasks;

    public class DislikeService : IDislikeService
    {
        private readonly IRepository repository;

        private readonly IGymService gymService;
        private readonly IUserService userService;

        public DislikeService (
            IRepository _repository,
            IGymService _gymService,
            IUserService _userService)
        {
            this.repository = _repository;

            this.gymService = _gymService;
            this.userService = _userService;
        }

        public async Task<Dislike> CreateDislikeAsync(string gymId, string userId)
        {
            Dislike? dislike = await this.repository.All<Dislike>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            Like? like = await this.repository.All<Like>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
               .FirstOrDefaultAsync();

            if (like != null && like.IsDeleted == false)
            {
                like.IsDeleted = true;
                like.DeletedOn = DateTime.UtcNow;
            }

            if (dislike == null)
            {
                dislike = new Dislike
                {
                    GymId = Guid.Parse(gymId),
                    UserId = Guid.Parse(userId),
                    CreatedOn = DateTime.UtcNow
                };

                await this.repository.AddAsync(dislike);
            }
            else
            {
                if (dislike.IsDeleted == true)
                {
                    dislike.IsDeleted = false;
                    dislike.DeletedOn = null;
                }
                else
                {
                    dislike.IsDeleted = true;
                    dislike.DeletedOn = DateTime.UtcNow;
                }
            }

            return dislike;
        }

        public async Task<bool> CheckIfUserDislikedGymAsync(string gymId, string userId)
        {
            Dislike? like = await this.repository.All<Dislike>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
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

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

        public LikeService (IRepository _repository)
        {
            this.repository = _repository;
        }

        public async Task<Like> CreateLikeAsync(string gymId, string userId)
        {
            Like? like = await this.repository.All<Like>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            Dislike? dislike = await this.repository.All<Dislike>(l => l.GymId == Guid.Parse(gymId) && l.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (dislike != null && dislike.IsDeleted == false)
            {
                dislike.IsDeleted = true;
                dislike.DeletedOn = DateTime.UtcNow;
            }

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
        
        public async Task<int> GetAllActiveLikesCountAsync()
        {
            return await this.repository.AllReadonly<Like>(l => l.IsDeleted == false)
                .CountAsync();
        }
    }
}

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

        public DislikeService (IRepository _repository)
        {
            this.repository = _repository;
        }

        public async Task<Dislike> CreateDislikeAsync(string gymId, string userId)
        {
            Dislike? dislike = await this.repository.All<Dislike>()
                .FirstOrDefaultAsync(l => l.GymId.ToString() == gymId && l.UserId.ToString() == userId);

            Like? like = await this.repository.AllNotDeleted<Like>()
               .FirstOrDefaultAsync(l => l.GymId.ToString() == gymId && l.UserId.ToString() == userId);

            if (like != null)
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

            await this.repository.SaveChangesAsync();

            return dislike;
        }

        public async Task DeleteDislikeAsync(string dislikeId)
        {
            Dislike? dislikeToDelete = await this.repository
                .AllNotDeleted<Dislike>()
                .FirstOrDefaultAsync(dl => dl.Id.ToString() == dislikeId);

            if (dislikeToDelete != null)
            {
                dislikeToDelete.IsDeleted = true;
                dislikeToDelete.DeletedOn = DateTime.UtcNow;
                
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckIfUserDislikedGymAsync(string gymId, string userId)
        {
            Dislike? like = await this.repository.AllReadonly<Dislike>()
                .FirstOrDefaultAsync(l => l.GymId.ToString() == gymId && l.UserId.ToString() == userId);

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

        public async Task<int> GetAllActiveDislikesCountAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Dislike>()
              .CountAsync();
        }
    }
}
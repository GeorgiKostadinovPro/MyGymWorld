namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;

    public interface ILikeService
    {
        Task<Like> CreateLikeAsync(string gymId, string userId);

        Task DeleteLikeAsync(string likeId);

        Task<bool> CheckIfUserLikedGymAsync(string gymId, string userId);
        
        Task<int> GetAllActiveLikesCountAsync();
    }
}

namespace MyGymWorld.Core.Contracts
{
    public interface ILikeService
    {
        Task CreateLikeAsync(string gymId, string userId);

        Task<bool> CheckIfUserLikedGymAsync(string gymId, string userId);
    }
}

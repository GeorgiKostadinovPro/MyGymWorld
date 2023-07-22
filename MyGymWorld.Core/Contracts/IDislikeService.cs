namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using System.Threading.Tasks;

    public interface IDislikeService
    {
        Task<Dislike> CreateDislikeAsync(string gymId, string userId);

        Task<bool> CheckIfUserDislikedGymAsync(string gymId, string userId);

        Task<int> GetAllActiveDislikesCountAsync();
    }
}

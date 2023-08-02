namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Comments;

    public interface ICommentService
    {
        Task CreateCommentAsync(string gymId, string userId, string content, string? parentId = null);

        Task DeleteCommentAsync(string commentId);

        Task<IEnumerable<CommentViewModel>> GetActiveCommentsByGymIdAsync(string gymId, int skip = 0, int? take = null);

        Task<int> GetActiveCommentsCountByGymIdAsync(string gymId);

        Task<int> GetAllActiveCommentsCountAsync();

        Task<Comment?> GetComentByIdAsync(string commentId);

        Task<bool> IsInSameGymByIdAsync(string commentId, string gymId);
    }
}

namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Comments;

    public interface ICommentService
    {
        Task CreateCommentAsync(string gymId, string userId, string content, string? parentId = null);

        Task<IEnumerable<CommentViewModel>> GetActiveCommentsByGymIdAsync(string gymId, int skip = 0, int? take = null);

        Task<int> GetAllActiveCommentsAsync();
    }
}

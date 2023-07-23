namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Comments;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private readonly IRepository repository;

        public CommentService(IRepository _repository)
        {
            this.repository = _repository;
        }

        public async Task CreateCommentAsync(string gymId, string userId, string content)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CommentViewModel>> GetActiveCommentsByGymIdAsync(string gymId, int skip = 0, int? take = null)
        {
            throw new NotImplementedException();
        }
    }
}

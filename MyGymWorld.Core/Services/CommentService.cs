namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
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

        public async Task CreateCommentAsync(string gymId, string userId, string content, string? parentId = null)
        {
            Comment comment = new Comment 
            { 
                GymId = Guid.Parse(gymId),
                UserId = Guid.Parse(userId),
                ParentId = parentId != null ? Guid.Parse(parentId) : null,
                Content = content,
                CreatedOn = DateTime.UtcNow
            };

            await this.repository.AddAsync(comment);
            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentViewModel>> GetActiveCommentsByGymIdAsync(string gymId, int skip = 0, int? take = null)
        {
            throw new NotImplementedException();
        }
    }
}

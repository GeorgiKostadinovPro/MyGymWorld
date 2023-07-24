namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Comments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        public CommentService(
            IMapper _mapper, 
            IRepository _repository)
        {
            this.mapper = _mapper;
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
            IQueryable<Comment> commentsAsQuery 
                = this.repository.AllReadonly<Comment>(c => c.IsDeleted == false)
                                 .Include(c => c.User)
                                 .Skip(skip);

            if (take.HasValue)
            {
                commentsAsQuery = commentsAsQuery.Take(take.Value);
            }

            return await commentsAsQuery
                .OrderByDescending(c => c.CreatedOn)
                .ProjectTo<CommentViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<int> GetActiveCommentsCountByGymIdAsync(string gymId)
        {
            return await this.repository.AllReadonly<Comment>(c => c.IsDeleted == false && c.GymId == Guid.Parse(gymId))
                .CountAsync();
        }

        public async Task<Comment?> GetComentByIdAsync(string commentId)
        {
            return await this.repository.AllReadonly<Comment>(c => c.IsDeleted == false && c.Id == Guid.Parse(commentId))
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetAllActiveCommentsCountAsync()
        {
            return await this.repository.AllReadonly<Comment>(c => c.IsDeleted == false)
                .CountAsync();
        }

        public async Task<bool> IsInSameGymByIdAsync(string commentId, string gymId)
        {
            Guid commentGymId = await this.repository
                .AllReadonly<Comment>(c => c.IsDeleted == false && c.Id == Guid.Parse(commentId))
                .Select(c => c.GymId)
                .FirstOrDefaultAsync();

            return commentGymId == Guid.Parse(gymId);
        }
    }
}

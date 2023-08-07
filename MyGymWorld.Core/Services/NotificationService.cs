namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Notifications;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class NotificationService : INotificationService
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        private readonly IUserService userService;

        public NotificationService(IRepository _repository, IMapper _mapper, IUserService _userService)
        {
            this.repository = _repository;
            this.mapper = _mapper;

            this.userService = _userService;
        }

        public async Task<Notification?> CreateNotificationAsync(string content, string url = null!, string userId = null!)
        {
            if (userId != null)
            {
                Notification notification = new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = content,
                    Url = url,
                    CreatedOn = DateTime.UtcNow
                };

                await this.repository.AddAsync(notification);
                await this.repository.SaveChangesAsync();

                return notification;
            }
            else
            {
                var users = await this.userService.GetAllAsync();

                foreach (var user in users)
                {
                    Notification notification = new Notification
                    {
                        UserId = user.Id,
                        Content = content,
                        Url = url,
                        CreatedOn = DateTime.UtcNow
                    };

                    await this.repository.AddAsync(notification);
                }

                await this.repository.SaveChangesAsync();
            }
            
            return await this.repository.AllNotDeletedReadonly<Notification>().LastOrDefaultAsync();
        }

        public async Task<Notification?> DeleteNotificationAsync(string notificationId)
        {
            Notification? notification = await this.repository
                .AllNotDeleted<Notification>()
                .FirstOrDefaultAsync(n => n.Id.ToString() == notificationId);

            if (notification != null)
            {
                notification.IsDeleted = true;
                notification.DeletedOn = DateTime.UtcNow;

                await this.repository.SaveChangesAsync();
            }

            return notification;
        }

        public async Task<Notification?> ReadNotificationAsync(string notificationId)
        {
            Notification? notification = await this.repository
              .AllNotDeleted<Notification>()
              .FirstOrDefaultAsync(n => n.Id.ToString() == notificationId);

            if (notification != null && notification.IsRead == false)
            {
                notification.IsRead = true;
                notification.ModifiedOn = DateTime.UtcNow;

                await this.repository.SaveChangesAsync();
            }

            return notification;
        }

        public async Task DeleteAllNotificationsByUserIdAsync(string userId)
        {
            IEnumerable<Notification> notificationsToDelete = await this.repository
                .AllNotDeleted<Notification>()
                .Where(n => n.UserId.ToString() == userId)
                .ToArrayAsync();

            if (notificationsToDelete.Any())
            {
                foreach (Notification notification in notificationsToDelete)
                {
                    notification.IsDeleted = true;
                    notification.DeletedOn = DateTime.UtcNow;
                }

                await this.repository.SaveChangesAsync();
            }
        }

        public async Task ReadAllNotificationsByUserIdAsync(string userId)
        {
            IEnumerable<Notification> notificationsToDelete = await this.repository
                .AllNotDeleted<Notification>()
                .Where(n => n.UserId.ToString() == userId && n.IsRead == false)
                .ToArrayAsync();

            if (notificationsToDelete.Any())
            {
                foreach (Notification notification in notificationsToDelete)
                {
                    notification.IsRead = true;
                    notification.ModifiedOn = DateTime.UtcNow;
                }

                await this.repository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetActiveNotificationsByUserIdAsync(string userId, int skip = 0, int? take = null)
        {
            IQueryable<Notification> notificationsAsQuery =
                this.repository.AllNotDeletedReadonly<Notification>()
                               .Where(n => n.UserId.ToString() == userId)
                               .OrderByDescending(n => n.CreatedOn)
                               .Skip(skip);

            if (take.HasValue)
            {
                notificationsAsQuery = notificationsAsQuery.Take(take.Value);
            }

            return await notificationsAsQuery
                        .ProjectTo<NotificationViewModel>(this.mapper.ConfigurationProvider)
                        .ToArrayAsync();
        }

        public async Task<int> GetUnReadNotificationsCountByUserIdAsync(string userId)
        {
            return await this.repository.AllNotDeletedReadonly<Notification>()
                                       .Where(n => n.UserId.ToString() == userId&& n.IsRead == false)
                                       .CountAsync();
        }

        public async Task<int> GetActiveNotificationsCountByUserIdAsync(string userId)
        {
            return await this.repository.AllNotDeletedReadonly<Notification>()
                                       .Where(n => n.UserId.ToString() == userId)
                                       .CountAsync();
        }
    } 
}
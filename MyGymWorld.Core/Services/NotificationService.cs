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

        public async Task<Notification?> CreateNotificationAsync(string content, string url, string userId = null)
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
            }

            await this.repository.SaveChangesAsync();

            return await this.repository.AllReadonly<Notification>().LastOrDefaultAsync();
        } 
        
        public async Task<Notification> DeleteNotificationAsync(string notificationId)
        {
            Notification notification = await this.repository
                .GetByIdAsync<Notification>(Guid.Parse(notificationId));

            notification.IsDeleted = true;
            notification.DeletedOn = DateTime.UtcNow;

            await this.repository.SaveChangesAsync();

            return notification;
        }

        public async Task<Notification> ReadNotificationByIdAsync(string notificationId)
        {
            Notification notification = await this.repository
               .GetByIdAsync<Notification>(Guid.Parse(notificationId));

            notification.IsRead = true;
            notification.ModifiedOn = DateTime.UtcNow;

            await this.repository.SaveChangesAsync();

            return notification;
        }

        public async Task<IEnumerable<NotificationViewModel>> GetFilteredNotificationsByUserIdAsync(string userId, bool isRead)
        {
            return await this.repository.AllReadonly<Notification>(n => n.IsDeleted == false)
                                        .Where(n => n.UserId == Guid.Parse(userId) && n.IsRead == isRead)
                                        .OrderByDescending(x => x.CreatedOn)
                                        .ProjectTo<NotificationViewModel>(this.mapper.ConfigurationProvider)
                                        .ToArrayAsync();
        }

        public async Task<IEnumerable<NotificationViewModel>> GetAllAsync(string userId)
        {
            return await this.repository.AllReadonly<Notification>(n => n.IsDeleted == false)
                                        .Where(n => n.UserId == Guid.Parse(userId))
                                        .OrderByDescending(x => x.CreatedOn)
                                        .ProjectTo<NotificationViewModel>(this.mapper.ConfigurationProvider)
                                        .ToArrayAsync();
        }

        public async Task<int> GetAllNotificationsCountByUserIdAsync(string userId)
        {
            return await this.repository.AllReadonly<Notification>( n => n.IsDeleted == false)
                                       .Where(n => n.UserId == Guid.Parse(userId))
                                       .CountAsync();
        }

        public async Task<int> GetUnReadNotificationsCountByUserIdAsync(string userId)
        {
            return await this.repository.AllReadonly<Notification>(n => n.IsDeleted == false)
                                       .Where(n => n.UserId == Guid.Parse(userId) && n.IsRead == false)
                                       .CountAsync();
        }
    }
}

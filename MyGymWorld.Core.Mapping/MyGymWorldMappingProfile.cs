namespace MyGymWorld.Core.Mapping
{
    using AutoMapper;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Web.ViewModels.Administration.Managers;
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using MyGymWorld.Web.ViewModels.Articles;
    using MyGymWorld.Web.ViewModels.Comments;
    using MyGymWorld.Web.ViewModels.Countries;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Gyms;
    using MyGymWorld.Web.ViewModels.Managers;
    using MyGymWorld.Web.ViewModels.Managers.Articles;
    using MyGymWorld.Web.ViewModels.Managers.Events;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using MyGymWorld.Web.ViewModels.Notifications;
    using MyGymWorld.Web.ViewModels.Users;
    using System.Linq;

    public class MyGymWorldMappingProfile : Profile
    {
        public MyGymWorldMappingProfile()
        {
            // Users
            this.CreateMap<RegisterUserInputModel, CreateUserInputModel>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Password, opt => opt.MapFrom(src => src.Password));

            this.CreateMap<CreateUserInputModel, ApplicationUser>();

            this.CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(d => d.ManagerId, opt => opt.MapFrom(src => src.Manager.Id))
                .ForMember(d => d.IsApproved, opt => opt.MapFrom(src => src.Manager.IsApproved))
                .ForMember(d => d.IsRejected, opt => opt.MapFrom(src => src.Manager.IsRejected))
                .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("dd/MM/yyyy h:mm tt")))
                .ForMember(d => d.DeletedOn, opt => opt.MapFrom(src => src.DeletedOn.HasValue
                ? src.DeletedOn.Value.ToString("dd/MM/yyyy h:mm tt")
                : null))
                .ForMember(d => d.Role, opt => opt.Ignore());

            this.CreateMap<ApplicationUser, UserProfileViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName != null ? src.FirstName : "None"))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName != null ? src.LastName : "None"))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber : "None"))
                .ForMember(d => d.ProfilePictureUri, opt => opt.MapFrom(src => src.ProfilePictureUri != null ? src.ProfilePictureUri : "None"))
                .ForMember(d => d.LikesCount, opt => opt.MapFrom(src => src.Likes.Count(l => l.IsDeleted == false)))  
                .ForMember(d => d.DislikesCount, opt => opt.MapFrom(src => src.Dislikes.Count(l => l.IsDeleted == false)))
                .ForMember(d => d.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count(l => l.IsDeleted == false)))
				.ForMember(d => d.EventsCount, opt => opt.MapFrom(src => src.UsersEvents.Count(l => l.IsDeleted == false)))
				.ForMember(d => d.Address, opt => opt.MapFrom(src => src.AddressId != null ? string.Concat(src.Address.Name, ", ", src.Address.Town.Name, ", ", src.Address.Town.Country.Name) : "None"));

            // Managers
            this.CreateMap<ApplicationUser, BecomeManagerInputModel>();

            this.CreateMap<Manager, ManagerRequestViewModel>()
                .ForMember(d => d.ManagerId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(d => d.ManagerProfilePictureUri, opt => opt.MapFrom(src => src.User.ProfilePictureUri))
                .ForMember(d => d.ManagerType, opt => opt.MapFrom(src => src.ManagerType.ToString()));

            // Roles
            this.CreateMap<ApplicationRole, RoleViewModel>()
                .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("dd/MM/yyyy h:mm tt")))
                .ForMember(d => d.DeletedOn, opt => opt.MapFrom(src => src.DeletedOn.HasValue
                ? src.DeletedOn.Value.ToString("dd/MM/yyyy h:mm tt")
                : null));

            this.CreateMap<ApplicationRole, EditRoleInputModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            // Notifications
            this.CreateMap<Notification, NotificationViewModel>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId.ToString()));

            // Gyms
            this.CreateMap<Gym, GymViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.Manager.User.UserName))
                 .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("d MMMM yyyy")));

            this.CreateMap<Gym, DisplayGymViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("d MMMM yyyy")))
                 .ForMember(d => d.TotalDays, opt => opt.MapFrom(src => (int)(DateTime.UtcNow - src.CreatedOn).TotalDays));

            this.CreateMap<Gym, GymDetailsViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.ManagerId, opt => opt.MapFrom(src => src.ManagerId.ToString()))
                 .ForMember(d => d.ManagerFullName, opt => opt.MapFrom(src => string.Concat(src.Manager.User.FirstName, " ", src.Manager.User.LastName)))
                 .ForMember(d => d.GymType, opt => opt.MapFrom(src => src.GymType.ToString()))
                 .ForMember(d => d.Address, opt => opt.MapFrom(src => string.Concat(src.Address.Name, ", ", src.Address.Town.Name, ", ", src.Address.Town.Country.Name)))
                 .ForMember(d => d.GymImages, opt => opt.MapFrom(src => src.GymImages.Select(gi => gi.Uri)))
                 .ForMember(d => d.UsersCount, opt => opt.MapFrom(src => src.UsersGyms.Count(ug => ug.IsDeleted == false)))
                 .ForMember(d => d.LikesCount, opt => opt.MapFrom(src => src.Likes.Count(l => l.IsDeleted == false)))
                 .ForMember(d => d.DislikesCount, opt => opt.MapFrom(src => src.Dislikes.Count(dl => dl.IsDeleted == false)))
                 .ForMember(d => d.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count(c => c.IsDeleted == false)))
                 .ForMember(d => d.ArticlesCount, opt => opt.MapFrom(src => src.Articles.Count(c => c.IsDeleted == false)))
                 .ForMember(d => d.TotalDays, opt => opt.MapFrom(src => (int)(DateTime.UtcNow - src.CreatedOn).TotalDays));

            // Comments
            this.CreateMap<Comment, CommentViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.ParentId, opt => opt.MapFrom(src => src.ParentId.HasValue ? src.ParentId.ToString() : null))
                 .ForMember(d => d.GymId, opt => opt.MapFrom(src => src.GymId.ToString()))
                 .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                 .ForMember(d => d.Author, opt => opt.MapFrom(src => src.User.UserName))
                 .ForMember(d => d.AuthorProfilePictureUri, opt => opt.MapFrom(src => src.User.ProfilePictureUri != null ? src.User.ProfilePictureUri : "https://img.freepik.com/free-icon/user_318-159711.jpg?w=2000"));

            // Events
            this.CreateMap<Event, EventViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.GymId, opt => opt.MapFrom(src => src.Gym.Id.ToString()))
                .ForMember(d => d.LogoUri, opt => opt.MapFrom(src => src.Gym.LogoUri))
                .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("d MMMM yyyy")));

            this.CreateMap<Event, EventDetailsViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
               .ForMember(d => d.GymId, opt => opt.MapFrom(src => src.Gym.Id.ToString()))
               .ForMember(d => d.GymName, opt => opt.MapFrom(src => src.Gym.Name))
               .ForMember(d => d.LogoUri, opt => opt.MapFrom(src => src.Gym.LogoUri))
               .ForMember(d => d.OrganiserId, opt => opt.MapFrom(src => src.Gym.Manager.Id.ToString()))
               .ForMember(d => d.EventType, opt => opt.MapFrom(src => src.EventType.ToString()))
               .ForMember(d => d.Organiser, opt => opt.MapFrom(src => string.Concat(src.Gym.Manager.User.FirstName, " ", src.Gym.Manager.User.LastName)))
			   .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("dd/MM/yyyy h:mm tt")));

            this.CreateMap<CreateEventInputModel, Event>()
                .ForMember(d => d.GymId, opt => opt.MapFrom(src => Guid.Parse(src.GymId)))
                .ForMember(d => d.EventType, opt => opt.MapFrom(src => Enum.Parse<EventType>(src.EventType)));

            this.CreateMap<Event, EditEventInputModel>()
              .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
              .ForMember(d => d.GymId, opt => opt.MapFrom(src => src.Gym.Id.ToString()))
              .ForMember(d => d.EventType, opt => opt.MapFrom(src => src.EventType.ToString()))
              .ForMember(d => d.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy h:mm tt")))
              .ForMember(d => d.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy h:mm tt")));

            // Articles
            this.CreateMap<CreateArticleInputModel, Article>()
                .ForMember(d => d.GymId, opt => opt.MapFrom(src => Guid.Parse(src.GymId)));

            this.CreateMap<Article, ArticleViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.GymId, opt => opt.MapFrom(src => src.Gym.Id.ToString()))
                .ForMember(d => d.GymName, opt => opt.MapFrom(src => src.Gym.Name))
                .ForMember(d => d.ShortContent, opt => opt.MapFrom(src => src.Content.Substring(0, 50)));

			this.CreateMap<Article, ArticleDetailsViewModel>()
			   .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
			   .ForMember(d => d.GymId, opt => opt.MapFrom(src => src.Gym.Id.ToString()))
			   .ForMember(d => d.GymName, opt => opt.MapFrom(src => src.Gym.Name))
			   .ForMember(d => d.LogoUri, opt => opt.MapFrom(src => src.Gym.LogoUri))
			   .ForMember(d => d.Content, opt => opt.MapFrom(src => src.Content));

			// Countries
			this.CreateMap<Country, CountryViewModel>();
        }
    }
}
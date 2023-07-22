﻿namespace MyGymWorld.Core.Mapping
{
    using AutoMapper;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Administration.Managers;
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using MyGymWorld.Web.ViewModels.Countries;
    using MyGymWorld.Web.ViewModels.Gyms;
    using MyGymWorld.Web.ViewModels.Managers;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using MyGymWorld.Web.ViewModels.Notifications;
    using MyGymWorld.Web.ViewModels.Users;
    using System.Linq;

    public class MyGymWorldMappingProfile : Profile
    {
        public MyGymWorldMappingProfile()
        {
            // Gyms
            this.CreateMap<Gym, GymViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.Manager.User.UserName))
                 .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("dd/MM/yyyy h:mm tt")));

            this.CreateMap<Gym, DisplayGymViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("dd/MM/yyyy h:mm tt")))
                 .ForMember(d => d.TotalDays, opt => opt.MapFrom(src => (int)(DateTime.UtcNow - src.CreatedOn).TotalDays));

            this.CreateMap<Gym, GymDetailsViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(d => d.ManagerId, opt => opt.MapFrom(src => src.ManagerId.ToString()))
                 .ForMember(d => d.ManagerFullName, opt => opt.MapFrom(src => string.Concat(src.Manager.User.FirstName, " ", src.Manager.User.LastName)))
                 .ForMember(d => d.GymType, opt => opt.MapFrom(src => src.GymType.ToString()))
                 .ForMember(d => d.Address, opt => opt.MapFrom(src => string.Concat(src.Address.Name, ", ", src.Address.Town.Name, ", ", src.Address.Town.Country.Name)))
                 .ForMember(d => d.GymImages, opt => opt.MapFrom(src => src.GymImages.Select(gi => gi.Uri)))
                 .ForMember(d => d.UsersCount, opt => opt.MapFrom(src => src.UsersGyms.Count(ug => ug.IsDeleted == false)))
                 .ForMember(d => d.LikesCount, opt => opt.MapFrom(src => src.Likes.Count(ug => ug.IsDeleted == false)))
                 .ForMember(d => d.DislikesCount, opt => opt.MapFrom(src => src.Dislikes.Count(ug => ug.IsDeleted == false)))
                 .ForMember(d => d.TotalDays, opt => opt.MapFrom(src => (int)(DateTime.UtcNow - src.CreatedOn).TotalDays));

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

            // Countries
            this.CreateMap<Country, CountryViewModel>();
        }
    }
}
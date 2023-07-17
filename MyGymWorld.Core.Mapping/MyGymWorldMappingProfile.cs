namespace MyGymWorld.Core.Mapping
{
    using AutoMapper;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Administration.Managers;
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using MyGymWorld.Web.ViewModels.Countries;
    using MyGymWorld.Web.ViewModels.Managers;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using MyGymWorld.Web.ViewModels.Notifications;
    using MyGymWorld.Web.ViewModels.Users;

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
                 .ForMember(d => d.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToString("dd/MM/yyyy h:mm tt")));

            // Countries
            this.CreateMap<Country, CountryViewModel>();
        }
    }
}

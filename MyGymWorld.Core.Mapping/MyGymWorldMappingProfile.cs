namespace MyGymWorld.Core.Mapping
{
    using AutoMapper;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Countries;
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

            // Countries
            this.CreateMap<Country, CountryViewModel>();
        }
    }
}

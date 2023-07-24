namespace MyGymWorld.Web.Infrastructure.Extensions
{
    using CloudinaryDotNet;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Core.Utilities.Services;
    using MyGymWorld.Data.Repositories;

    public static class MyGymWorldServiceCollectionExtensions
    {   
        /// <summary>
        /// This is an extension method for the IServiceCollection interface provided by Microsoft.
        /// This method keeps and adds all different services among the application.
        /// It keeps them in one place in order not to make the Program.cs file overflow.
        /// </summary>
        /// <param name="services">Using IServiceCollection interface for extension</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Repository pattern
            services.AddScoped<IRepository, Repository>();

            // SendGrid services
            services.AddTransient<IEmailSenderService, EmailSenderService>();

            // Cloudinary account
            Account cloudinaryAccount = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:APIKey"],
                configuration["Cloudinary:APISecret"]);

            // Cloudinary services
            Cloudinary cloudinary = new Cloudinary(cloudinaryAccount);

            services.AddSingleton(cloudinary);
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Authentication services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IManagerService, ManagerService>();

            // standard services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IGymService, GymService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IDislikeService, DislikeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ITownService, TownService>();
            services.AddScoped<IAddressService, AddressService>();

            return services;
        }
    }
}
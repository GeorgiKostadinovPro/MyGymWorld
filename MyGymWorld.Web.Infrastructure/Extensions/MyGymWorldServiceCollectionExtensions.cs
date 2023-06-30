namespace MyGymWorld.Web.Infrastructure.Extensions
{
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
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();

            services.AddTransient<IEmailSenderService, EmailSenderService>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
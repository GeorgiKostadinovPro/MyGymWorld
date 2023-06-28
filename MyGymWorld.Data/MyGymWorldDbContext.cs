using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyGymWorld.Data.Models;
using System.Reflection;

namespace MyGymWorld.Data
{
    public class MyGymWorldDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public MyGymWorldDbContext(DbContextOptions<MyGymWorldDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(MyGymWorldDbContext))
                ?? Assembly.GetExecutingAssembly();

            var configurationTypes = assembly
                .GetTypes()
                .Where(t => t.GetInterfaces()
                             .Any(i => i.IsGenericType && i.GetType() == typeof(IEntityTypeConfiguration<>)))
                .ToList();

            foreach ( var configurationType in configurationTypes)
            {
                dynamic configurationInstance = Activator.CreateInstance(configurationType);

                if (configurationInstance)
                {
                    builder.ApplyConfiguration(configurationInstance);
                }
            }

            base.OnModelCreating(builder);
        }
    }
}

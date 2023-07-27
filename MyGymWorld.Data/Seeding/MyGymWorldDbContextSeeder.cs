using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyGymWorld.Data.Seeding.Contracts;

namespace MyGymWorld.Data.Seeding
{
    public class MyGymWorldDbContextSeeder : ISeeder
    {
        public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(typeof(MyGymWorldDbContextSeeder));

            var seeders = new List<ISeeder>
                          {
                              new RolesSeeder(),
                              new CountriesSeeder(),
                              new TownsSeeder(),
                              new CategoriesSeeder()
                          };

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext, serviceProvider);
                await dbContext.SaveChangesAsync();
                logger?.LogInformation($"Seeder {seeder.GetType().Name} done.");
            }
        }
    }
}

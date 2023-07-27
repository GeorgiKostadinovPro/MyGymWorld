namespace MyGymWorld.Data.Seeding
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Seeding.Contracts;
    using System;
    using System.Threading.Tasks;

    public class CountriesSeeder : ISeeder
    {
        public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Countries.AnyAsync())
            {
                return;
            }

            IEnumerable<Country> countries = new HashSet<Country>()
            {
                new Country()
                {
                    Name = "Bulgaria",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "United States",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "United Kingdom",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "Russia",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "South Korea",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "China",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "France",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "Spain",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "Germany",
                    CreatedOn = DateTime.UtcNow,
                },
                new Country()
                {
                    Name = "Italy",
                    CreatedOn = DateTime.UtcNow,
                },
                //new Country()
                //{
                //    Name = "Portual",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Netherlands",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Canada",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Sweden",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Norway",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "South Korea",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Poland",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Romania",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "Serbia",
                //    CreatedOn = DateTime.UtcNow,
                //},
                //new Country()
                //{
                //    Name = "India",
                //    CreatedOn = DateTime.UtcNow,
                //}
            };

            await dbContext.Countries.AddRangeAsync(countries);
            await dbContext.SaveChangesAsync();
        }
    }
}

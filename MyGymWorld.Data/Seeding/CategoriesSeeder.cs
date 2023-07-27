namespace MyGymWorld.Data.Seeding
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Seeding.Contracts;
    using System;
    using System.Threading.Tasks;

    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            IEnumerable<Category> catgeories = new HashSet<Category>
            {
                new Category
                {
                    Name = "Nutrition & Wellness"
                },
                new Category
                {
                    Name = "Fitness Tech & Gear"
                },
                new Category
                {
                    Name = "Health & Mindfulness"
                },
                new Category
                {
                    Name = "Athlete Spotlights"
                },
                new Category
                {
                    Name = "Business & Jobs Offers"
                }
            };

            await dbContext.Categories.AddRangeAsync(catgeories);
            await dbContext.SaveChangesAsync();
        }
    }
}

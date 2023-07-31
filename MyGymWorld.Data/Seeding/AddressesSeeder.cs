namespace MyGymWorld.Data.Seeding
{
	using Microsoft.EntityFrameworkCore;
	using MyGymWorld.Data.Models;
	using MyGymWorld.Data.Seeding.Contracts;
	using System;
	using System.Threading.Tasks;

	public class AddressesSeeder : ISeeder
	{
		public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
		{
			if (await dbContext.Addresses.AnyAsync())
			{
				return;
			}

			Town town = await dbContext.Towns.FirstAsync(t => t.Name == "Sofia");

			Address address = new Address 
			{ 
				Name = "bul. Cherni vrah",
				TownId = town.Id,
				CreatedOn = DateTime.UtcNow
			};

			await dbContext.Addresses.AddAsync(address);
			await dbContext.SaveChangesAsync();
		}
	}
}

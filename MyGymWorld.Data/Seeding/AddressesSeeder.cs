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

			IEnumerable<Address> addresses = new HashSet<Address>
			{
                 new Address
                 {
                     Name = "bul. Cherni vrah",
                     TownId = town.Id,
                     CreatedOn = DateTime.UtcNow
                 },
				 new Address
				 {
					 Name = "bul. Bulgaria 1",
                     TownId = town.Id,
                     CreatedOn = DateTime.UtcNow
                 },
				 new Address
				 {
                     Name = "bul. Bulgaria 3",
                     TownId = town.Id,
                     CreatedOn = DateTime.UtcNow
                 },
				 new Address
				 {
                     Name = "Mladost 4",
                     TownId = town.Id,
                     CreatedOn = DateTime.UtcNow
                 }
			};
		   
			await dbContext.Addresses.AddRangeAsync(addresses);
			await dbContext.SaveChangesAsync();
		}
	}
}

namespace MyGymWorld.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using MyGymWorld.Data.Models;
	using MyGymWorld.Data.Seeding.Contracts;
	using System;
	using System.Threading.Tasks;

	public class ManagersSeeder : ISeeder
	{
		public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
		{
			UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await dbContext.Managers.AnyAsync())
			{
				return;
			}

			ApplicationUser user = await userManager.FindByEmailAsync("manager@gmail.com");
			
			Manager manager = new Manager 
			{ 
				UserId = user.Id,
				IsApproved = true,
				ManagerType = Models.Enums.ManagerType.ManyGymsManager,
				CreatedOn = DateTime.UtcNow
			};

			await dbContext.Managers.AddAsync(manager);

			user.ManagerId = manager.Id;

			await userManager.UpdateAsync(user);
		}
	}
}

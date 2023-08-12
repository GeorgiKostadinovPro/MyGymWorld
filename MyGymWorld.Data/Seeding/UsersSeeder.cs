namespace MyGymWorld.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using MyGymWorld.Common;
	using MyGymWorld.Data.Models;
	using MyGymWorld.Data.Seeding.Contracts;
	using System;
	using System.Threading.Tasks;

	public class UsersSeeder : ISeeder
	{
		public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
		{
			UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await dbContext.Users.AnyAsync())
			{
				return;
			}

			Address address = await dbContext.Addresses.FirstAsync(a => a.Name == "bul. Cherni vrah");

			ICollection<Notification> notifications = new HashSet<Notification>
			{
				new Notification
				{
					Content = "This is a test notification!",
					CreatedOn = DateTime.UtcNow
				},
				new Notification
				{
					Content = "This is a read notification!",
					IsRead = true,
					CreatedOn = DateTime.UtcNow
				}
			};

			ApplicationUser admin = new ApplicationUser
			{
				UserName = "Admin",
				PasswordHash = "AdminPass123.",
				Email = "mgmwrlddmnccnt@gmail.com",
				EmailConfirmed = true,
				CreatedOn = DateTime.UtcNow,
				Notifications = notifications
			};

			ApplicationUser manager = new ApplicationUser
			{
				UserName = "Manager",
				FirstName = "Gosho",
				LastName = "Petrov",
				PhoneNumber = "+359-879454529",
				AddressId = address.Id,
				PasswordHash = "ManagerPass123.",
				Email = "manager@gmail.com",
				EmailConfirmed = true,
				CreatedOn = DateTime.UtcNow,
                Notifications = notifications
            };

			ApplicationUser user = new ApplicationUser
			{
				UserName = "Go4ko",
				PasswordHash = "UserPass123.",
				Email = "user@gmail.com",
				EmailConfirmed = true,
				CreatedOn = DateTime.UtcNow,
                Notifications = notifications
            };

			await userManager.CreateAsync(admin, admin.PasswordHash);
			await userManager.CreateAsync(manager, manager.PasswordHash);
			await userManager.CreateAsync(user, user.PasswordHash);

			await userManager.AddToRoleAsync(admin, ApplicationRoleConstants.AdministratorRoleName);
			await userManager.AddToRoleAsync(manager, ApplicationRoleConstants.ManagerRoleName);
		}
	}
}

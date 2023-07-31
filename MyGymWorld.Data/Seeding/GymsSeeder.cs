namespace MyGymWorld.Data.Seeding
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using MyGymWorld.Data.Models;
	using MyGymWorld.Data.Seeding.Contracts;
	using System;
	using System.Threading.Tasks;

	public class GymsSeeder : ISeeder
	{
		public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
		{
			UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await dbContext.Gyms.AnyAsync())
			{
				return;
			}

			ApplicationUser user = await userManager.FindByEmailAsync("manager@gmail.com");
			Address address = await dbContext.Addresses.FirstAsync(a => a.Name == "bul. Cherni vrah");

			// The images are already pushed to Cloudinary

			Gym gym1 = new Gym
			{
				Name = "Vibes Health & Fitness",
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Description = "The best gym in Sofia and around!",
				WebsiteUrl = "https://www.vibesfit.com/",
				GymType = Models.Enums.GymType.PowerLifting,
				LogoUri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690805077/MyGymWorld/assets/gyms-logo-pictures/VibesHealthFitness_twrizb.jpg",
				LogoPublicId = "VShred-gym-logo_fc2mlg",
				ManagerId = user.ManagerId.Value,
				AddressId = address.Id,
			};

			await dbContext.Gyms.AddAsync(gym1);
			await dbContext.SaveChangesAsync();
		}
	}
}

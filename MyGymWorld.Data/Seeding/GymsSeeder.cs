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

			// The images are already pushed to Cloudinary the publicIds are exposed, but you cannot perform any actions without my OWN APIKey and APISecret

			IEnumerable<Gym> gyms = new HashSet<Gym>
			{
				new Gym
			    {
			    	Name = "Vibes Health & Fitness",
			    	Email = user.Email,
			    	PhoneNumber = user.PhoneNumber,
			    	Description = "The best gym in Sofia and around!",
			    	WebsiteUrl = "https://www.vibesfit.com/",
			    	GymType = Models.Enums.GymType.PowerLifting,
			    	LogoUri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690805535/MyGymWorld/assets/gyms-logo-pictures/VShred-gym-logo_fc2mlg.jpg",
			    	LogoPublicId = "VShred-gym-logo_fc2mlg",
			    	ManagerId = user.ManagerId.Value,
			    	AddressId = address.Id,
					CreatedOn = DateTime.UtcNow,
			    	GymImages = new HashSet<GymImage>
			    	{
			    		new GymImage
			    		{
			    			Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690805535/MyGymWorld/assets/gyms-logo-pictures/VShred-gym-logo_fc2mlg.jpg",
			    			PublicId = "VShred-gym-logo_fc2mlg",
							CreatedOn = DateTime.UtcNow
			    		},
			    		new GymImage
			    		{
			    			Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690806513/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image_ffshwx.jpg",
			    			PublicId = "gym-gallery-image_ffshwx",
							CreatedOn = DateTime.UtcNow
						},
						new GymImage
						{
							Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690806858/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-2_tofeaa.jpg",
							PublicId = "gym-gallery-image-2_tofeaa",
							CreatedOn = DateTime.UtcNow
						},
						new GymImage
						{
							Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690806960/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-3_xnrz6u.png",
							PublicId = "gym-gallery-image-3_xnrz6u",
							CreatedOn = DateTime.UtcNow
						}
			    	}
			    }
			};

			await dbContext.Gyms.AddRangeAsync(gyms);
			await dbContext.SaveChangesAsync();
		}
	}
}

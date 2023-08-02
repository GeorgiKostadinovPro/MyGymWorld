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

			ApplicationUser managerUser = await userManager.FindByEmailAsync("manager@gmail.com");
			ApplicationUser normalUser = await userManager.FindByEmailAsync("user@gmail.com");
			ApplicationUser admin = await userManager.FindByEmailAsync("mgmwrlddmnccnt@gmail.com");

			Address address1 = await dbContext.Addresses.FirstAsync(a => a.Name == "bul. Cherni vrah");
            Address address2 = await dbContext.Addresses.FirstAsync(a => a.Name == "bul. Bulgaria 1");
            Address address4 = await dbContext.Addresses.FirstAsync(a => a.Name == "Mladost 4");


            // The images are already pushed to Cloudinary the publicIds are exposed, but you cannot perform any actions without my OWN APIKey and APISecret

            IEnumerable<Gym> gyms = new HashSet<Gym>
			{
				new Gym
			    {
			    	Name = "VShred",
			    	Email = managerUser.Email,
			    	PhoneNumber = managerUser.PhoneNumber,
			    	Description = "The best gym in Sofia and around!",
			    	WebsiteUrl = "https://www.vibesfit.com/",
			    	GymType = Models.Enums.GymType.PowerLifting,
			    	LogoUri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690805535/MyGymWorld/assets/gyms-logo-pictures/VShred-gym-logo_fc2mlg.jpg",
			    	LogoPublicId = "VShred-gym-logo_fc2mlg",
			    	ManagerId = managerUser.ManagerId.Value,
			    	AddressId = address1.Id,
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
			    	},
					Events = new HashSet<Event>
					{
						new Event
						{
							Name = "Vshred Meeting",
							Description = "Meet with our trainers and managers.",
							StartDate = DateTime.UtcNow,
							EndDate = DateTime.UtcNow.AddDays(1),
							EventType = Models.Enums.EventType.Conference,
							CreatedOn = DateTime.UtcNow
						},
                        new Event
                        {
                            Name = "Vshred Training",
                            Description = "Meet with our trainers and managers to train and gain experience.",
                            StartDate = DateTime.UtcNow.AddDays(4),
                            EndDate = DateTime.UtcNow.AddDays(10),
                            EventType = Models.Enums.EventType.Training,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Event
                        {
                            Name = "Vshred Opening",
                            Description = "Come and open the gym with us!",
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(1),
                            EventType = Models.Enums.EventType.Training,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
					Articles = new HashSet<Article>
					{
						new Article
						{
							Title = "Test Article VShred",
							Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
							"Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of" +
							" type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic " +
							"typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
							"and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
							CreatedOn = DateTime.UtcNow
                        },
						new Article
						{
                            Title = "Test Article 2 VShred",
                            Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
                            "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of" +
                            " type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic " +
                            "typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
                            "and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                            CreatedOn = DateTime.UtcNow
                        }
					},
					Comments = new HashSet<Comment>
					{
						new Comment
						{
							Content = "Very good gym!",
							UserId = normalUser.Id,
							CreatedOn = DateTime.UtcNow
						},
                        new Comment
                        {
                            Content = "Excellent Gym!",
                            UserId = admin.Id,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
					Likes = new HashSet<Like>
					{
						new Like
						{
							UserId = normalUser.Id,
							CreatedOn = DateTime.UtcNow
                        },
						new Like
						{
							UserId = admin.Id,
							CreatedOn = DateTime.UtcNow
						}
					},
					Memberships = new HashSet<Membership>
					{
						new Membership
						{
							Price = 25.00M,
							MembershipType = Models.Enums.MembershipType.Week,
							CreatedOn = DateTime.UtcNow
						},
                        new Membership
                        {
                            Price = 50.00M,
                            MembershipType = Models.Enums.MembershipType.Month,
                            CreatedOn = DateTime.UtcNow
                        }
                    }
			    },
                new Gym
                {
                    Name = "Next Level",
                    Email = managerUser.Email,
                    PhoneNumber = managerUser.PhoneNumber,
                    Description = "The best gym for crossfiters!",
                    WebsiteUrl = "https://www.nextlevelclub.bg/",
                    GymType = Models.Enums.GymType.CrossFit,
                    LogoUri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690990480/MyGymWorld/assets/gyms-logo-pictures/NextLevel-gym-logo_r0fkl9.jpg",
                    LogoPublicId = "NextLevel-gym-logo_r0fkl9",
                    ManagerId = managerUser.ManagerId.Value,
                    AddressId = address2.Id,
                    CreatedOn = DateTime.UtcNow,
                    GymImages = new HashSet<GymImage>
                    {
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690990480/MyGymWorld/assets/gyms-logo-pictures/NextLevel-gym-logo_r0fkl9.jpg",
                            PublicId = "NextLevel-gym-logo_r0fkl9",
                            CreatedOn = DateTime.UtcNow
                        },
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690990707/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-4_ojbenb.jpg",
                            PublicId = "gym-gallery-image-4_ojbenb",
                            CreatedOn = DateTime.UtcNow
                        }, 
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690990706/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-5_sancmq.jpg",
                            PublicId = "gym-gallery-image-5_sancmq",
                            CreatedOn = DateTime.UtcNow
                        },
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690990706/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-6_wxetgk.jpg",
                            PublicId = "gym-gallery-image-6_wxetgk",
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Events = new HashSet<Event>
                    {
                        new Event
                        {
                            Name = "Next Level Meeting",
                            Description = "Meet with our trainers and managers.",
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddDays(1),
                            EventType = Models.Enums.EventType.Conference,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Event
                        {
                            Name = "Next Level Training",
                            Description = "Meet with our trainers and managers to train and gain experience.",
                            StartDate = DateTime.UtcNow.AddDays(4),
                            EndDate = DateTime.UtcNow.AddDays(10),
                            EventType = Models.Enums.EventType.Training,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Articles = new HashSet<Article>
                    {
                        new Article
                        {
                            Title = "Test Article Next Level",
                            Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
                            "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of" +
                            " type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic " +
                            "typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
                            "and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Article
                        {
                            Title = "Test Article 2 Next Level",
                            Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
                            "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of" +
                            " type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic " +
                            "typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
                            "and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Comments = new HashSet<Comment>
                    {
                        new Comment
                        {
                            Content = "Very good gym!",
                            UserId =normalUser.Id,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Comment
                        {
                            Content = "Excellent Gym!",
                            UserId = admin.Id,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Likes = new HashSet<Like>
                    {
                        new Like
                        {
                            UserId = normalUser.Id,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Memberships = new HashSet<Membership>
                    {
                        new Membership
                        {
                            Price = 30.00M,
                            MembershipType = Models.Enums.MembershipType.Week,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Membership
                        {
                            Price = 80.00M,
                            MembershipType = Models.Enums.MembershipType.Month,
                            CreatedOn = DateTime.UtcNow
                        }
                    }
                },
                 new Gym
                {
                    Name = "Gorilla Gym",
                    Email = managerUser.Email,
                    PhoneNumber = managerUser.PhoneNumber,
                    Description = "The best gym for weightlifters!",
                    WebsiteUrl = "https://easybook.bg/fitnes-tsentar-gorilla-gym",
                    GymType = Models.Enums.GymType.PowerLifting,
                    LogoUri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690992108/MyGymWorld/assets/gyms-logo-pictures/GorillaGym-gym-logo_xy721f.jpg",
                    LogoPublicId = "GorillaGym-gym-logo_xy721f",
                    ManagerId = managerUser.ManagerId.Value,
                    AddressId = address4.Id,
                    CreatedOn = DateTime.UtcNow,
                    GymImages = new HashSet<GymImage>
                    {
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690992108/MyGymWorld/assets/gyms-logo-pictures/GorillaGym-gym-logo_xy721f.jpg",
                            PublicId = "GorillaGym-gym-logo_xy721f",
                            CreatedOn = DateTime.UtcNow
                        },
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690992541/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-7_xkqqig.jpg",
                            PublicId = "gym-gallery-image-7_xkqqig",
                            CreatedOn = DateTime.UtcNow
                        },
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690992541/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-8_j7oti2.jpg",
                            PublicId = "gym-gallery-image-8_j7oti2",
                            CreatedOn = DateTime.UtcNow
                        },
                        new GymImage
                        {
                            Uri = "https://res.cloudinary.com/de1i8aava/image/upload/v1690992541/MyGymWorld/assets/gyms-gallery-pictures/gym-gallery-image-9_kycr5e.jpg",
                            PublicId = "gym-gallery-image-9_kycr5e",
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Events = new HashSet<Event>
                    {
                        new Event
                        {
                            Name = "Gorilla Gym Meeting",
                            Description = "Meet with our trainers and managers.",
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddDays(1),
                            EventType = Models.Enums.EventType.Conference,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Event
                        {
                            Name = "Gorilla Gym Training",
                            Description = "Meet with our trainers and managers to train and gain experience.",
                            StartDate = DateTime.UtcNow.AddDays(2),
                            EndDate = DateTime.UtcNow.AddDays(10),
                            EventType = Models.Enums.EventType.Training,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Articles = new HashSet<Article>
                    {
                        new Article
                        {
                            Title = "Test Article Gorilla Gym",
                            Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
                            "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of" +
                            " type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic " +
                            "typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
                            "and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                            CreatedOn = DateTime.UtcNow
                        },
                        new Article
                        {
                            Title = "Test Article 2 Gorilla Gym",
                            Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
                            "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of" +
                            " type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic " +
                            "typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, " +
                            "and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Comments = new HashSet<Comment>
                    {
                        new Comment
                        {
                            Content = "Very good gym!",
                            UserId = normalUser.Id,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Comment
                        {
                            Content = "Totally recommend it!",
                            UserId = admin.Id,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Likes = new HashSet<Like>
                    {
                        new Like
                        {
                            UserId = normalUser.Id,
                            CreatedOn = DateTime.UtcNow
                        }
                    },
                    Memberships = new HashSet<Membership>
                    {
                        new Membership
                        {
                            Price = 15.00M,
                            MembershipType = Models.Enums.MembershipType.Week,
                            CreatedOn = DateTime.UtcNow
                        },
                        new Membership
                        {
                            Price = 100.00M,
                            MembershipType = Models.Enums.MembershipType.Month,
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

namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Moq;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class GymServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;

        private Mock<IEventService> eventServiceMock;
        private Mock<IArticleService> articleServiceMock;
        private Mock<IMembershipService> membershipServiceMock;
        private Mock<ILikeService> likeServiceMock;
        private Mock<IDislikeService> dislikeServiceMock;
        private Mock<ICommentService> commentServiceMock;
        private Mock<IAddressService> addressServiceMock;

        [SetUp]
        public async Task Setup()
        {
            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.eventServiceMock = new Mock<IEventService>();
            this.articleServiceMock = new Mock<IArticleService>();
            this.membershipServiceMock = new Mock<IMembershipService>();
            this.likeServiceMock = new Mock<ILikeService>();
            this.dislikeServiceMock = new Mock<IDislikeService>();
            this.commentServiceMock = new Mock<ICommentService>();
            this.addressServiceMock = new Mock<IAddressService>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task GetActiveGymsCountAsyncShouldWorkProperly()
        {
            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetActiveGymsCountAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetActiveGymsCountAsyncShouldReturnZeroWhenThereAreNoActiveGyms()
        {
            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetActiveGymsCountAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllUserJoinedGymsCountAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = true,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = false,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = false,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetAllUserJoinedGymsCountAsync(userId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllUserJoinedGymsCountAsyncShouldReturnZeroWhenIdIsInvalid(string userId)
        {
            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = true,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = false,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetAllUserJoinedGymsCountAsync(userId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllUsersWhoAreSubscribedForGymArticlesAsyncShouldWorkProperly()
        {
            var userOneId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userTwoId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse(userOneId),
                    Email = "test1@gmail.com",
                    UserName = "Test1",
                    EmailConfirmed = true,
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = Guid.Parse(userTwoId),
                    Email = "test2@gmail.com",
                    UserName = "Test2",
                    EmailConfirmed = true,
                    IsDeleted = false
                }
            });

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userOneId),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = true,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.Parse(userTwoId),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = false,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = false,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetAllUsersWhoAreSubscribedForGymArticlesAsync(gymId);

            Assert.That(result.Count(), Is.EqualTo(1));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ApplicationUser));
        }

        [Test]
        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("632fe39a-bc5b-4ea4-b0c5-68b2da06768e", false)]
        public async Task GetAllUsersWhoAreSubscribedForGymArticlesAsyncShouldReturnEmptyCollectionWhenParamsAreInvalid(string gymId, bool isSubscribed)
        {
            var userOneId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userTwoId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var validatedGymId = !string.IsNullOrWhiteSpace(gymId)
                ? gymId
                : "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse(userOneId),
                    Email = "test1@gmail.com",
                    UserName = "Test1",
                    EmailConfirmed = true,
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = Guid.Parse(userTwoId),
                    Email = "test2@gmail.com",
                    UserName = "Test2",
                    EmailConfirmed = true,
                    IsDeleted = false
                }
            });

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userOneId),
                    GymId = Guid.Parse(validatedGymId),
                    IsSubscribedForArticles = isSubscribed,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.Parse(userTwoId),
                    GymId = Guid.Parse(validatedGymId),
                    IsSubscribedForArticles = isSubscribed,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.Parse(validatedGymId),
                    IsSubscribedForArticles = isSubscribed,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetAllUsersWhoAreSubscribedForGymArticlesAsync(gymId);

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCase(
           "732fe39a-bc5b-4ea4-b0c5-68b2da06768e",
           "832fe39a-bc5b-4ea4-b0c5-68b2da06768e",
           true)]
        [TestCase(
           "732fe39a-bc5b-4ea4-b0c5-68b2da06768e",
           "832fe39a-bc5b-4ea4-b0c5-68b2da06768e",
           false)]
        public async Task CheckIfUserIsSubscribedForGymArticlesShouldWorkProperly(string userId, string gymId, bool isSubscribed)
        {
            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = isSubscribed,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = false,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfUserIsSubscribedForGymArticles(userId, gymId);

            Assert.That(result, Is.EqualTo(isSubscribed));
        }

        [Test]
        public async Task CheckIfGymExistsByIdAsyncShouldReturnTrueWhenGymExists()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymId),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfGymExistsByIdAsync(gymId);

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfGymExistsByIdAsyncShouldReturnFalseWhenGymDoesNotExists(string gymId)
        {
            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfGymExistsByIdAsync(gymId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CheckIfGymIsManagedByManagerAsyncShouldReturnTrueWheGymIsManagedByTheGivenManager()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var managerId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymId),
                    ManagerId = Guid.Parse(managerId),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfGymIsManagedByManagerAsync(gymId, managerId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CheckIfGymIsManagedByManagerAsyncShouldReturnFalseWheGymIsNotManagedByTheGivenManager()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var managerId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    ManagerId = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfGymIsManagedByManagerAsync(gymId, managerId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CheckIfGymIsJoinedByUserAsyncShouldReturnTrueWhenGymIsJoinedByTheGivenUser()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = true,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = false,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = false,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfGymIsJoinedByUserAsync(gymId, userId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CheckIfGymIsJoinedByUserAsyncShouldReturnFalseWhenGymIsNotJoinedByTheGivenUser()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsSubscribedForArticles = false,
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    IsSubscribedForArticles = false,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.CheckIfGymIsJoinedByUserAsync(gymId, userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetGymByIdAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymId),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetGymByIdAsync(gymId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(gymId));
        }


        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetGymByIdAsyncShouldReturnNullWhenIdIsInvalid(string gymId)
        {
            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = false
                },
                new Gym
                {
                    Id = Guid.NewGuid(),
                    Email = "test@gmail.com",
                    PhoneNumber = "1234567890",
                    Name = "Gym Test",
                    Description = "Gym Test",
                    LogoUri = "test",
                    LogoPublicId = "test",
                    WebsiteUrl = "test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetGymByIdAsync(gymId);

            Assert.IsNull(result);
        }

        [Test]
        public void GetAllGymTypesShouldWorkProperly()
        {
            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = service.GetAllGymTypes();

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}

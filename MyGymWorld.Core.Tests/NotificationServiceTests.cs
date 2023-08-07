namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Moq;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using System.Threading.Tasks;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Web.ViewModels.Notifications;
    using System;
    using Microsoft.EntityFrameworkCore;

    [TestFixture]
    public class NotificationServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;
        
        private Mock<IUserService> userServiceMock;

        [SetUp]
        public async Task Setup()
        {
            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.userServiceMock = new Mock<IUserService>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateNotificationAsyncForOneUserShouldWorkProperly()
        {
            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                Email = "test@gmail.com",
                UserName = "Test",
                EmailConfirmed = true,
                IsDeleted = false
            };

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Notification>()))
                .Callback(async (Notification notification) =>
                {
                    await this.dbContext.Notifications.AddAsync(notification);
                    await this.dbContext.SaveChangesAsync();
                });

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Notification>())
               .Returns(this.dbContext.Notifications.Where(n => n.IsDeleted == false));

            var service = new NotificationService(
               this.mockRepository.Object,
               this.mapper,
               this.userServiceMock.Object);

            var result = await service.CreateNotificationAsync("Test", "/Test/Test", "932fe39a-bc5b-4ea4-b0c5-68b2da06768e");

            var notificationsCount = await this.dbContext
                .Notifications.CountAsync(x => x.IsDeleted == false);

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Notification>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.That(result.Content, Is.EqualTo("Test"));
            Assert.That(notificationsCount, Is.EqualTo(3));
        }

        [Test]
        public async Task CreateNotificationAsyncForAllUsersShouldWorkProperly()
        {
            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Email = "test1@gmail.com",
                    UserName = "Test1",
                    EmailConfirmed = true,
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Email = "test2@gmail.com",
                    UserName = "Test2",
                    EmailConfirmed = true,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Notification>()))
                .Callback(async (Notification notification) =>
                {
                    await this.dbContext.Notifications.AddAsync(notification);
                    await this.dbContext.SaveChangesAsync();
                });

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Notification>())
                .Returns(this.dbContext.Notifications.Where(n => n.IsDeleted == false));

            this.userServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(await this.dbContext.Users.ToArrayAsync());

            var service = new NotificationService(
               this.mockRepository.Object,
               this.mapper,
               this.userServiceMock.Object);

            var result = await service.CreateNotificationAsync("Test", "/Test/Test", null!);

            var notificationsCount = await this.dbContext
                .Notifications.CountAsync(x => x.IsDeleted == false);

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Notification>()), Times.AtLeast(2));
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.That(result.Content, Is.EqualTo("Test"));
            Assert.That(notificationsCount, Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteNotificationAsyncShouldWorkProperly()
        {
            string notificationId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    Id = Guid.Parse(notificationId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
                this.mockRepository.Object,
                this.mapper,
                this.userServiceMock.Object);

            var result = await service.DeleteNotificationAsync(notificationId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.That(result.IsDeleted, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task DeleteNotificationAsyncShouldNotDoAnythingWhenIdIsInvalidIrIsReadIsTrue(string notificationId)
        {
            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    Id = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
                this.mockRepository.Object,
                this.mapper,
                this.userServiceMock.Object);

            var result = await service.DeleteNotificationAsync(notificationId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);

            Assert.IsNull(result);
        }

        [Test]
        public async Task ReadNotificationAsyncShouldWorkProperly()
        {
            string notificationId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    Id = Guid.Parse(notificationId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test", 
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
                this.mockRepository.Object,
                this.mapper,
                this.userServiceMock.Object);

            var result = await service.ReadNotificationAsync(notificationId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.That(result.IsRead, Is.True);
        }

        [Test]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", true)]
        [TestCase(null, false)]
        public async Task ReadNotificationAsyncShouldNotDoAnythingWhenIdIsInvalidIrIsReadIsTrue(string notificationId, bool isRead)
        {
            string id = notificationId != null ? notificationId : "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    Id = Guid.Parse(id),
                    Content = "Test",
                    IsRead = isRead,
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    IsRead = false,
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    Id = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
                this.mockRepository.Object,
                this.mapper,
                this.userServiceMock.Object);

            var result = await service.ReadNotificationAsync(notificationId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAllNotificationsByUserIdAsyncShouldWorkProperly()
        {
            string userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    IsRead = false,
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = true
                },
                new Notification
                {
                    UserId = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
              .Setup(x => x.AllNotDeleted<Notification>())
              .Returns(this.dbContext.Notifications
              .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
              this.mockRepository.Object,
              this.mapper,
              this.userServiceMock.Object);

            await service.DeleteAllNotificationsByUserIdAsync(userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ReadAllNotificationsByUserIdAsyncShouldWorkProperly()
        {
            string userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    IsRead = false,
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
              .Setup(x => x.AllNotDeleted<Notification>())
              .Returns(this.dbContext.Notifications
              .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
              this.mockRepository.Object,
              this.mapper,
              this.userServiceMock.Object);

            await service.ReadAllNotificationsByUserIdAsync(userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", 0, 2)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", 1, 1)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", 0, null)]
        public async Task GetActiveNotificationsByUserIdAsyncShouldWorkProperly(string userId, int skip, int? take = null)
        {
            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    IsRead = false,
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
              this.mockRepository.Object,
              this.mapper,
              this.userServiceMock.Object);

            var result = await service.GetActiveNotificationsByUserIdAsync(userId, skip, take);

            int count = take.HasValue ? take.Value : (await this.dbContext.Notifications
               .Where(c => c.IsDeleted == false && c.UserId.ToString() == userId).CountAsync());

            Assert.That(result.Count(), Is.EqualTo(count));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(NotificationViewModel));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetActiveNotificationsByUserIdAsyncShouldReturnEmptyCollectionWhenUserIdIsInvalid(string userId)
        {
            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    IsRead = false,
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
              this.mockRepository.Object,
              this.mapper,
              this.userServiceMock.Object);

            var result = await service.GetActiveNotificationsByUserIdAsync(userId, 1, 2);

            Assert.That(result.Count(), Is.EqualTo(0));
        }


        [Test]
        public async Task GetUnReadNotificationsCountByUserIdAsyncShouldWorkProperly()
        {
            string userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    IsRead = false,
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = true
                },
                new Notification
                {
                    UserId = Guid.NewGuid(),
                    Content = "Test",
                    IsRead = false,
                    IsDeleted = false,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
                this.mockRepository.Object,
                this.mapper,
                this.userServiceMock.Object);

            var result = await service.GetActiveNotificationsCountByUserIdAsync(userId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllNotificationsCountByUserIdAsyncShouldWorkProperly()
        {
            string userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Notifications.AddRangeAsync(new HashSet<Notification>
            {
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsDeleted = false
                },
                new Notification
                {
                    UserId = Guid.Parse(userId),
                    Content = "Test",
                    IsDeleted = true
                },
                new Notification
                {
                    UserId = Guid.NewGuid(),
                    Content = "Test",
                    IsDeleted = false,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Notification>())
                .Returns(this.dbContext.Notifications
                .Where(n => n.IsDeleted == false));

            var service = new NotificationService(
                this.mockRepository.Object,
                this.mapper,
                this.userServiceMock.Object);

            var result = await service.GetActiveNotificationsCountByUserIdAsync(userId);

            Assert.That(result, Is.EqualTo(2));
        }
    }
}
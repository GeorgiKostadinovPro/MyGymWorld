namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Administration.Managers;
    using MyGymWorld.Web.ViewModels.Managers;
    using System.Threading.Tasks;

    [TestFixture]
    public class ManagerServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;

        private Mock<IUserService> userServiceMock;
        private Mock<IRoleService> roleServiceMock;
        private Mock<INotificationService> notificationServiceMock;

        [SetUp]
        public async Task Setup()
        {
            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.userServiceMock = new Mock<IUserService>();
            this.roleServiceMock = new Mock<IRoleService>();
            this.notificationServiceMock = new Mock<INotificationService>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateManagerAsyncShouldWorkProperly()
        {
            string userId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            string adminId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            string phoneNumber = "+359-879454529";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser> 
            {
                new ApplicationUser
                {
                    Id = Guid.Parse(userId),
                    UserName = "User",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = Guid.Parse(adminId),
                    UserName = "Manager",
                    IsDeleted = false
                }           
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Manager>()))
                .Callback(async (Manager manager) =>
                {
                    await this.dbContext.Managers.AddAsync(manager);
                    await this.dbContext.SaveChangesAsync();
                });

            this.userServiceMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(await this.dbContext.Users.FirstAsync(u => u.Id.ToString() == userId));

            this.userServiceMock
                .Setup(x => x.GetAdministratorAsync())
                .ReturnsAsync(await this.dbContext.Users.FirstAsync(u => u.Id.ToString() == adminId));

            this.notificationServiceMock
                .Setup(x => x.CreateNotificationAsync("", "", ""))
                .Callback(() => {});

            BecomeManagerInputModel becomeManagerInputModel = new BecomeManagerInputModel
            {
                Id = userId,
                UserName = "User",
                Email = "user@gmail.com",
                FirstName = "User",
                LastName = "User",
                PhoneNumber = phoneNumber,
                IsApproved = false,
                ManagerType = "OneGymManager"
            };
            
            var service = new ManagerService(
              this.mapper, this.mockRepository.Object,
              this.userServiceMock.Object, this.roleServiceMock.Object,
              this.notificationServiceMock.Object);

            await service.CreateManagerAsync(userId, becomeManagerInputModel);

            var managersCount = await this.dbContext.Managers.CountAsync(m => m.IsDeleted == false);

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Manager>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(managersCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ApproveManagerAsyncShouldWorkProperly()
        {
            var managerId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var adminId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                FirstName = "Test",
                LastName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.Parse(managerId),
                    UserId = Guid.Parse(userId),
                    IsApproved = false,
                    IsRejected = true,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
               this.mapper, this.mockRepository.Object,
               this.userServiceMock.Object, this.roleServiceMock.Object,
               this.notificationServiceMock.Object);

            var result = await service.ApproveManagerAsync(managerId, adminId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            this.roleServiceMock.Verify(x => x.AddRoleToUserAsync(userId, ApplicationRoleConstants.ManagerRoleName), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsApproved);
            Assert.IsFalse(result.IsRejected);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task ApproveManagerAsyncShouldThrowExceptionWhenManagerDoesNotExist(string managerId)
        {
            var adminId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                FirstName = "Test",
                LastName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
               this.mapper, this.mockRepository.Object,
               this.userServiceMock.Object, this.roleServiceMock.Object,
               this.notificationServiceMock.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var result = await service.ApproveManagerAsync(managerId, adminId);

            }, string.Format(ExceptionConstants.ManagerErrors.InvalidManagerId, managerId));
        }

        [Test]
        public async Task RejectManagerAsyncShouldWorkProperlyWhenManagerIsRejectedForTheFirstTime()
        {
            var managerId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var adminId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                FirstName = "Test",
                LastName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.Parse(managerId),
                    UserId = Guid.Parse(userId),
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
               this.mapper, this.mockRepository.Object,
               this.userServiceMock.Object, this.roleServiceMock.Object,
               this.notificationServiceMock.Object);

            var result = await service.RejectManagerAsync(managerId, adminId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsApproved);
            Assert.IsTrue(result.IsRejected);
        }

        [Test]
        public async Task RejectManagerAsyncShouldWorkProperlyWhenManagerWasApprovedBefore()
        {
            var managerId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var adminId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                FirstName = "Test",
                LastName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.Parse(managerId),
                    UserId = Guid.Parse(userId),
                    IsApproved = true,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            this.roleServiceMock
                .Setup(x => x.CheckIfUserIsInRoleAsync(userId, ApplicationRoleConstants.ManagerRoleName))
                .ReturnsAsync(true);

            var service = new ManagerService(
               this.mapper, this.mockRepository.Object,
               this.userServiceMock.Object, this.roleServiceMock.Object,
               this.notificationServiceMock.Object);

            var result = await service.RejectManagerAsync(managerId, adminId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            this.roleServiceMock.Verify(x => x.RemoveRoleFromUserAsync(userId, ApplicationRoleConstants.ManagerRoleName), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsApproved);
            Assert.IsTrue(result.IsRejected);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task RejectManagerAsyncShouldThrowExceptionWhenManagerDoesNotExist(string managerId)
        {
            var adminId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                FirstName = "Test",
                LastName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
               this.mapper, this.mockRepository.Object,
               this.userServiceMock.Object, this.roleServiceMock.Object,
               this.notificationServiceMock.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var result = await service.RejectManagerAsync(managerId, adminId);

            }, string.Format(ExceptionConstants.ManagerErrors.InvalidManagerId, managerId));
        }

        [Test]
        public async Task GetAllNotApprovedManagerRequestsAsyncShouldWorkProperly()
        {
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = true,
                    IsRejected = false,
                    IsDeleted = false,
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetAllNotApprovedManagerRequestsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ManagerRequestViewModel));
        }

        [Test]
        public async Task GetAllNotApprovedManagerRequestsCountAsyncShouldWorkProperly()
        {
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsApproved = true,
                    IsRejected = false,
                    IsDeleted = false,
                },
                new Manager
                {
                    IsApproved = false,
                    IsRejected = false,
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetAllNotApprovedManagerRequestsCountAsync();

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task GetSingleManagerRequestByManagerIdAsyncShouldWorkProperly()
        {
            string managerId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.Parse(managerId),
                    IsDeleted = false
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetSingleManagerRequestByManagerIdAsync(managerId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ManagerRequestViewModel>(result);
        }

        [Test]
        public async Task CheckIfUserIsAManagerAsyncShouldReturnTrueWhenUserIsAManager()
        {
            string userId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    UserId = Guid.Parse(userId),
                    IsDeleted = false
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.CheckIfUserIsAManagerAsync(userId);

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfUserIsAManagerAsyncShouldReturnFalseWhenUserIdIsInvalid(string userId)
        {
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.CheckIfUserIsAManagerAsync(userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task CheckIfManagerExistsByPhoneNumberAsyncShouldWorkProperly()
        {
            string userId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            string phoneNumber = "+359-879454529";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                PhoneNumber = phoneNumber,
                IsDeleted = false
            });
    
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    UserId = Guid.Parse(userId),
                    IsApproved = true,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsDeleted = false,
                },
                new Manager
                {
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.CheckIfManagerExistsByPhoneNumberAsync(phoneNumber);

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("+359-123456789")]
        public async Task CheckIfManagerExistsByPhoneNumberAsyncShouldReturnFalseWhenPhoneNumberIsInvalid(string phoneNumber)
        {
            string userId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "Manager",
                PhoneNumber = "+359-879454529",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    UserId = Guid.Parse(userId),
                    IsApproved = true,
                    IsRejected = false,
                    IsDeleted = false
                },
                new Manager
                {
                    IsDeleted = false,
                },
                new Manager
                {
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.CheckIfManagerExistsByPhoneNumberAsync(phoneNumber);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetManagerForApprovalAndRejectionAsyncShouldWorkProperly()
        {
            string managerId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.Parse(managerId),
                    IsDeleted = false
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetManagerForApprovalAndRejectionAsync(managerId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(managerId));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetManagerForApprovalAndRejectionAsyncShoudlReturnNullWhenIdIsInvalid(string managerId)
        {
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                 new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetManagerForApprovalAndRejectionAsync(managerId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetManagerByUserIdShouldWorkProperly()
        {
            string userId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    UserId = Guid.Parse(userId),
                    IsDeleted = false
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetManagerByUserIdAsync(userId);

            Assert.IsNotNull(result);
            Assert.That(result.UserId.ToString(), Is.EqualTo(userId));
        }


        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetManagerByUserIdShouldReturnNullWhenUserIdIsInvalid(string userId)
        {
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetManagerByUserIdAsync(userId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetManagerByIdAsyncShouldWorkProperly()
        {
            string managerId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.Parse(managerId),
                    IsDeleted = false
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetManagerByIdAsync(managerId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(managerId));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetManagerByIdAsyncShouldReturnNullWhenIdIsInvalid(string managerId)
        {
            await this.dbContext.Managers.AddRangeAsync(new HashSet<Manager>
            {
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                },
                new Manager
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Manager>())
                .Returns(this.dbContext.Managers
                .Where(m => m.IsDeleted == false));

            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = await service.GetManagerByIdAsync(managerId);

            Assert.IsNull(result);
        }

        [Test]
        public void GetAllManagerTypesShouldWorkProperly()
        {
            var service = new ManagerService(
                this.mapper, this.mockRepository.Object,
                this.userServiceMock.Object, this.roleServiceMock.Object,
                this.notificationServiceMock.Object);

            var result = service.GetAllManagerTypes();

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(string));
        }
    }
}
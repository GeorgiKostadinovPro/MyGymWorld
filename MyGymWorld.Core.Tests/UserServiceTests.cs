namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Moq;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using System.Threading.Tasks;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Core.Services;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Web.ViewModels.Users;
    using Microsoft.EntityFrameworkCore;
    using static QRCoder.PayloadGenerator;
    using MyGymWorld.Common;

    [TestFixture]
    public class UserServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private Mock<UserManager<ApplicationUser>> mockUserManager;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;

        private Mock<IAddressService> addressServiceMock;
        private Mock<ITownService> townServicemock;

        [SetUp]
        public async Task Setup()
        {
            this.mockUserManager = new Mock<UserManager<ApplicationUser>>
               (new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.addressServiceMock = new Mock<IAddressService>();
            this.townServicemock = new Mock<ITownService>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateUserAsyncShouldWorkProperly()
        {
            string password = "12345";

            this.mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
                .Callback(async (ApplicationUser user, string password) =>
                {
                    await this.dbContext.Users.AddAsync(user);
                    await this.dbContext.SaveChangesAsync();
                });

            CreateUserInputModel createUserInputModel = new CreateUserInputModel
            {
                UserName = "Test",
                Email = "user@gmail.com",
                Password = password
            };

            var service = new UserService(this.mockUserManager.Object,
               this.mapper, this.mockRepository.Object,
               this.addressServiceMock.Object, this.townServicemock.Object);

            var result = await service.CreateUserAsync(createUserInputModel);

            var createdUser = await this.dbContext.Users.FirstAsync();

            Assert.IsNotNull(createdUser);
            Assert.That(createdUser.Email, Is.EqualTo(createUserInputModel.Email));
        }

        [Test]
        public async Task EditUserAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse(userId),
                    UserName = "Manager",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            EditUserInputModel editUserInputModel = new EditUserInputModel
            {
                UserName = "Manager",
                Email = "user%gmail.com"
            };

            await service.EditUserAsync(userId, editUserInputModel);

            this.mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task EditUserAsyncShouldThrowExceptionWhenUserDoesNotExist(string userId)
        {
            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Manager",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.EditUserAsync(userId, new EditUserInputModel());

            }, ExceptionConstants.UserErros.InvalidUserId);
        }

        [Test]
        public async Task DeleteUserAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse(userId),
                    UserName = "Manager",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            await service.DeleteUserAsync(userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task DeleteUserAsyncShouldNotDoAnythingWhenUserDoesNotExist(string userId)
        {
            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Manager",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            await service.DeleteUserAsync(userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetUserByEmailAsyncShouldWorkProperly()
        {
            string email = "user@gmail.com";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Manager",
                    Email = email,
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockUserManager
                .Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(await this.dbContext.Users
                .FirstAsync(u => u.Email == email));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            var result = await service.GetUserByEmailAsync(email);

            Assert.IsNotNull(result);
            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public async Task CheckIfUserExistsByPhoneNumberAsyncShouldReturnTrueWhenUserExistsByPhoneNumber()
        {
            string phoneNumber = "1234567890";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Manager",
                    PhoneNumber = phoneNumber,
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            var result = await service.CheckIfUserExistsByPhoneNumberAsync(phoneNumber);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CheckIfUserExistsByPhoneNumberAsyncShouldReturnFalseWhenUserDoesNotExistsByPhoneNumber()
        {
            string phoneNumber = "1234567890";

            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Manager",
                    PhoneNumber = "1234567899",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            var result = await service.CheckIfUserExistsByPhoneNumberAsync(phoneNumber);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllAsyncShouldWorkProperly()
        {
            await this.dbContext.Users.AddRangeAsync(new HashSet<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Manager",
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    UserName = "Regular",
                    IsDeleted = true
                }    
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<ApplicationUser>())
                .Returns(this.dbContext.Users
                .Where(u => u.IsDeleted == false));

            var service = new UserService(this.mockUserManager.Object,
                this.mapper, this.mockRepository.Object,
                this.addressServiceMock.Object, this.townServicemock.Object);

            var result = await service.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ApplicationUser));
        }
    }
}

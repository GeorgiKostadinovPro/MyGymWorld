namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using NUnit.Framework;

    [TestFixture]
    public class RoleServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<ApplicationRole>> mockRoleManager;

        private Mock<IRepository> mockRepository;
        private IMapper mapper;   

        [SetUp]
        public async Task Setup()
        {
            this.mockUserManager = new Mock<UserManager<ApplicationUser>>
                (new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            this.mockRoleManager = new Mock<RoleManager<ApplicationRole>>
                (new Mock<IRoleStore<ApplicationRole>>().Object, null, null, null, null);

            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task GetRoleForEditAsyncShouldWorkProperly()
        {
            ApplicationRole role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = "Admin"
            };

            await this.dbContext.Roles.AddAsync(role);
            await this.dbContext.SaveChangesAsync();

            this.mockRoleManager
                .Setup(x => x.FindByIdAsync(role.Id.ToString()))
                .ReturnsAsync(role);

            var service = new RoleService(
               this.mockRoleManager.Object,
               this.mockUserManager.Object,
               this.mockRepository.Object,
               this.mapper);

            var result = await service.GetRoleForEditAsync(role.Id.ToString());

            Assert.IsNotNull(result);
            Assert.That(result, Is.TypeOf<EditRoleInputModel>());
        }

        [Test]
        [TestCase(0, 2)]
        [TestCase(1, 1)]
        [TestCase(2, 0)]
        public async Task GetActiveOrDeletedForAdministrationAsyncShouldReturnNotDeleted(int skip, int take)
        {
            await this.dbContext.Roles.AddRangeAsync(new HashSet<ApplicationRole>() 
            {
                new ApplicationRole
                {
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            var service = new RoleService(
                this.mockRoleManager.Object,
                this.mockUserManager.Object,
                this.mockRepository.Object,
                mapper);

            var result = await service.GetActiveOrDeletedForAdministrationAsync(false, skip, take);

            Assert.That(result.Count(), Is.EqualTo(take));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(RoleViewModel));
        }

        [Test]
        [TestCase(0, 2)]
        [TestCase(1, 1)]
        [TestCase(2, 0)]
        public async Task GetActiveOrDeletedForAdministrationAsyncShouldReturnDeleted(int skip, int take)
        {
            await this.dbContext.Roles.AddRangeAsync(new HashSet<ApplicationRole>()
            {
                new ApplicationRole
                {
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    IsDeleted = true
                },
                new ApplicationRole
                {
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            var service = new RoleService(
                this.mockRoleManager.Object,
                this.mockUserManager.Object,
                this.mockRepository.Object,
                mapper);

            var result = await service.GetActiveOrDeletedForAdministrationAsync(true, skip, take);

            Assert.That(result.Count(), Is.EqualTo(take));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(RoleViewModel));
        }

        [Test]
        public async Task CreateRoleAsyncshouldWorkProperly()
        {
            CreateRoleInputModel createRoleInputModel = new CreateRoleInputModel
            {
                Name = "Admin"
            };

            HashSet<ApplicationRole> roles = new HashSet<ApplicationRole>();

            this.mockRoleManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationRole>()))
                .Callback((ApplicationRole role) => roles.Add(role));

            var service = new RoleService(
                this.mockRoleManager.Object,
                this.mockUserManager.Object,
                this.mockRepository.Object,
                this.mapper);

            await service.CreateRoleAsync(createRoleInputModel);

            Assert.That(roles.Count, Is.EqualTo(1));
        }


        [Test]
        public async Task DeleteRoleAsyncShouldWorkProperly()
        {
            ApplicationRole manager = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                IsDeleted = false
            };

            await this.dbContext.Roles.AddRangeAsync(new HashSet<ApplicationRole>() { manager });
            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            this.mockUserManager
                .Setup(x => x.Users)
                .Returns(this.dbContext.Users.AsQueryable());

            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            await service.DeleteRoleAsync(manager.Id.ToString());

            Assert.IsTrue(manager.IsDeleted);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task DeleteRoleAsyncShouldDoNothingWhenIdIsinvalid(string roleId)
        {
            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            await service.DeleteRoleAsync(roleId);

            var count = this.mockRepository.Invocations.Count;

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task EditRoleAsyncshouldWorkProperly()
        {
            var roleId = Guid.NewGuid();
            var newName = "AdminEdited";

            EditRoleInputModel editRoleInputModel = new EditRoleInputModel
            {
                Name = newName
            };

            ApplicationRole role = new ApplicationRole
            {
                Id = roleId,
                Name = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            this.mockRoleManager
                .Setup(x => x.FindByIdAsync(roleId.ToString()))
                .ReturnsAsync(role);

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.EditRoleAsync(roleId.ToString(), editRoleInputModel);

            this.mockRoleManager.Verify(x => x.FindByIdAsync(roleId.ToString()), Times.Once);
            this.mockRoleManager.Verify(x => x.UpdateAsync(role), Times.Once);

            Assert.That(role.Name, Is.EqualTo(newName));
        }

        [Test]
        public async Task EditRoleAsyncShouldNotUpdateRoleWhenRoleNotFound()
        {
            EditRoleInputModel editRoleInputModel = new EditRoleInputModel
            {
                Name = "Admin"
            };

            ApplicationRole role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            string roleId = Guid.NewGuid().ToString();

            this.mockRoleManager
                .Setup(x => x.FindByIdAsync(roleId))
                .ReturnsAsync((ApplicationRole)null);

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.EditRoleAsync(roleId, editRoleInputModel);

            this.mockRoleManager.Verify(x => x.FindByIdAsync(roleId.ToString()), Times.Once);
            this.mockRoleManager.Verify(m => m.UpdateAsync(It.IsAny<ApplicationRole>()), Times.Never);

            int count = this.mockRoleManager.Invocations.Count;

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task EditRoleAsyncShouldNotDoAnythingWhenIdIsNullOrEmpty(string roleId)
        {
            EditRoleInputModel editRoleInputModel = new EditRoleInputModel { Name = "New Role Name" };

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.EditRoleAsync(roleId, editRoleInputModel);

            var count = this.mockRepository.Invocations.Count;

            this.mockRoleManager.Verify(m => m.FindByIdAsync(roleId), Times.Never);
            this.mockRoleManager.Verify(m => m.UpdateAsync(It.IsAny<ApplicationRole>()), Times.Never);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task AddRoleToUserAsyncShouldWorkProperly()
        {
            ApplicationRole role = new ApplicationRole
            {
                Name = "Admin"
            };

            var userId = Guid.NewGuid();

            ApplicationUser user = new ApplicationUser
            {
                Id = userId
            };

            this.mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.AddRoleToUserAsync(userId.ToString(), role.Name);

            var count = this.mockUserManager.Invocations.Count;

            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        [TestCase("", "Admin")]
        [TestCase(null, "Admin")]
        [TestCase("1234", "")]
        [TestCase("1234", null)]
        public async Task AddRoleToUserAsyncShouldNotDoAnythingWhenParamsAreInvalid(string userId, string roleName)
        {
            ApplicationRole role = new ApplicationRole
            {
                Name = "Admin"
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.NewGuid()
            };

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.AddRoleToUserAsync(userId, roleName);

            this.mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Never);
            this.mockUserManager.Verify(x => x.AddToRoleAsync(user, roleName), Times.Never);

            var count = this.mockUserManager.Invocations.Count;

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task RemoveRoleFromUserAsyncShouldWorkProperly()
        {
            ApplicationRole role = new ApplicationRole
            {
                Name = "Admin"
            };

            var userId = Guid.NewGuid();

            ApplicationUser user = new ApplicationUser
            {
                Id = userId
            };

            this.mockUserManager
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.RemoveRoleFromUserAsync(userId.ToString(), role.Name);

            this.mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
            this.mockUserManager.Verify(x => x.RemoveFromRoleAsync(user, role.Name), Times.Once);

            int count = this.mockUserManager.Invocations.Count;

            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        [TestCase("", "Admin")]
        [TestCase(null, "Admin")]
        [TestCase("1234", "")]
        [TestCase("1234", null)]
        public async Task RemoveRoleFromUserAsyncShouldNotDoAnythingWhenParamsAreInvalid(string userId, string roleName)
        {
            ApplicationRole role = new ApplicationRole
            {
                Name = "Admin"
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.NewGuid()
            };

            var service = new RoleService(
              this.mockRoleManager.Object,
              this.mockUserManager.Object,
              this.mockRepository.Object,
              this.mapper);

            await service.RemoveRoleFromUserAsync(userId, roleName);

            this.mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Never);
            this.mockUserManager.Verify(x => x.RemoveFromRoleAsync(user, roleName), Times.Never);

            int count = this.mockUserManager.Invocations.Count;

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetActiveOrDeletedRolesCountShouldReturnNotDeleted()
        {
            await this.dbContext.Roles.AddRangeAsync(new HashSet<ApplicationRole>()
            {
                new ApplicationRole
                {
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            var count = await service.GetActiveOrDeletedRolesCountAsync(false);

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetActiveOrDeletedRolesCountShouldReturnDeleted()
        {
            await this.dbContext.Roles.AddRangeAsync(new HashSet<ApplicationRole>()
            {
                new ApplicationRole
                {
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    IsDeleted = true
                },
                new ApplicationRole
                {
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            var count = await service.GetActiveOrDeletedRolesCountAsync(true);

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task CheckIfRoleAlreadyExistsByIdAsyncShouldWorkProperly()
        {
            var roleId = Guid.NewGuid();

            ApplicationRole role = new ApplicationRole
            {
                Id = roleId
            };

            this.mockRoleManager
                .Setup(x => x.FindByIdAsync(roleId.ToString()))
                .ReturnsAsync(role);

            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            var result = await service.CheckIfRoleAlreadyExistsByIdAsync(roleId.ToString());

            Assert.That(result, Is.True);
        }


        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfRoleAlreadyExistsByIdAsyncShouldReturnFalseWhenIdIsInvalid(string roleId)
        {
            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            var result = await service.CheckIfRoleAlreadyExistsByIdAsync(roleId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CheckIfRoleAlreadyExistsByNameAsyncShouldWorkProperly()
        {
            await this.dbContext.Roles.AddAsync(new ApplicationRole
            {
                Name = "Admin"
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            var result = await service.CheckIfRoleAlreadyExistsByNameAsync("Admin");

            Assert.That(result, Is.True);
        }


        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task CheckIfRoleAlreadyExistsByNameAsyncShouldReturnFalseWhenRoleNameIsInvalid(string roleName)
        {
            await this.dbContext.Roles.AddAsync(new ApplicationRole
            {
                Name = "Admin"
            });

            await this.dbContext.SaveChangesAsync();

            var service = new RoleService(
             this.mockRoleManager.Object,
             this.mockUserManager.Object,
             this.mockRepository.Object,
             this.mapper);

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<ApplicationRole>())
                .Returns(this.dbContext.Roles.AsQueryable());

            var result = await service.CheckIfRoleAlreadyExistsByNameAsync(roleName);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CheckIfUserIsInRoleAsyncShouldWorkProperly()
        {
            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.NewGuid()
            };

            ApplicationRole role = new ApplicationRole
            {
                Name = "Admin"
            };

            this.mockUserManager
                .Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            var service = new RoleService(
               this.mockRoleManager.Object,
               this.mockUserManager.Object,
               this.mockRepository.Object,
               this.mapper);

            var result = await service.CheckIfUserIsInRoleAsync(user.Id.ToString(), "Admin");

            this.mockUserManager.Verify(x => x.IsInRoleAsync(user, "Admin"), Times.Once);
        }

        [Test]
        [TestCase("", "Admin")]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "")]
        [TestCase(null, "Admin")]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", null)]
        public async Task CheckIfUserIsInRoleAsyncShouldReturnFalseWhenParamsAreInvalid(string userId, string roleName)
        {
            var service = new RoleService(
                this.mockRoleManager.Object,
                this.mockUserManager.Object,
                this.mockRepository.Object,
                this.mapper);

            var result = await service.CheckIfUserIsInRoleAsync(userId, roleName);

            Assert.That(result, Is.False);
        }
    }
}
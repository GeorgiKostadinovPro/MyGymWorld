namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Moq;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using System.Threading.Tasks;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Web.ViewModels.Managers.Memberships;
    using MyGymWorld.Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using System;

    [TestFixture]
    public class MembershipServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;

        private Mock<IQRCoderService> qrCodeServiceMock;

        [SetUp]
        public async Task Setup()
        {
            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.qrCodeServiceMock = new Mock<IQRCoderService>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateMembershipAsyncShouldWorkProperly()
        {
            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Membership>()))
                .Callback(async (Membership membership) =>
                {
                    await this.dbContext.Memberships.AddAsync(membership);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);
                
            CreateMembershipInputModel createMembershipInputModel = new CreateMembershipInputModel
            {
                GymId = Guid.NewGuid().ToString(),
                Price = 10m,
                MembershipType = "Week"
            };

            var result = await service.CreateMembershipAsync(createMembershipInputModel);
            
            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Membership>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Membership>(result);
            Assert.That(result.MembershipType.ToString(), Is.EqualTo(createMembershipInputModel.MembershipType));
        }

        [Test]
        public async Task EditMembershipAsyncShouldWorkProperly()
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership> 
            { 
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            EditMembershipInputModel editMembershipInputModel = new EditMembershipInputModel
            {
                Id = membershipId,
                GymId = Guid.NewGuid().ToString(),
                Price = 10m,
                MembershipType = "Month"
            };

            await service.EditMembershipAsync(membershipId, editMembershipInputModel);

            var editedMembership = await this.dbContext.Memberships
                .FirstAsync(m => m.Id.ToString() == membershipId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(editedMembership.MembershipType.ToString(), Is.EqualTo("Month"));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task EditMembershipAsyncShouldNotDoAnythingWhenIdIsInvalid(string membershipId)
        {
            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            EditMembershipInputModel editMembershipInputModel = new EditMembershipInputModel
            {
                Id = membershipId,
                GymId = Guid.NewGuid().ToString(),
                Price = 10m,
                MembershipType = "Month"
            };

            await service.EditMembershipAsync(membershipId, editMembershipInputModel);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteMembershipAsyncShouldWorkProperly()
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            await service.DeleteMembershipAsync(membershipId);

            var deletedMembership = await this.dbContext.Memberships
                .FirstAsync(m => m.Id.ToString() == membershipId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            Assert.IsTrue(deletedMembership.IsDeleted);
        }


        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task DeleteMembershipAsyncShouldNotDoAnythingWhenIdIsInvalid(string membershipId)
        {
            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService( this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);
               
            await service.DeleteMembershipAsync(membershipId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetMembershipDetailsByIdAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            
            await this.dbContext.Gyms.AddAsync(new Gym
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
            });

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    GymId = Guid.Parse(gymId),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetMembershipDetailsByIdAsync(membershipId);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(membershipId));
        }

        [Test]
        public async Task GetMembershipForEditShouldWorkProperly()
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    GymId = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetMembershipForEditByIdAsync(membershipId);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(membershipId));
        }

        [Test]
        public async Task GetUserMembershipAsyncShouldWorkProperly()
        {
            var userId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.Parse(userId),
                    MembershipId = Guid.Parse(membershipId),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.NewGuid(),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserMembership>())
                .Returns(this.dbContext.UsersMemberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetUserMembershipAsync(userId, membershipId);

            Assert.IsNotNull(result);
            Assert.That(result.UserId.ToString(), Is.EqualTo(userId));
        }

        [Test]
        [TestCase(null, "932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", "932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetUserMembershipAsyncShouldReturnNullWhenNotFound(string userId, string membershipId)
        {
            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.NewGuid(),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserMembership>())
                .Returns(this.dbContext.UsersMemberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetUserMembershipAsync(userId, membershipId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetMembershipByIdAsyncShouldWorkProperly()
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetMembershipByIdAsync(membershipId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(membershipId));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetMembershipByIdAsyncShouldReturnNullWhenIdIsInvalid(string membershipId)
        {
            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetMembershipByIdAsync(membershipId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task CheckIfMembershipExistsByIdAsyncShouldReturnTrueWhenMembershipExists()
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.CheckIfMembershipExistsByIdAsync(membershipId);

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfMembershipExistsByIdAsyncShouldReturnFalseWhenMembershipDoesNotExist(string membershipId)
        {
            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.CheckIfMembershipExistsByIdAsync(membershipId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllActiveMembershipsCountAsyncShouldWorkProperly()
        {
            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Week,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = await service.GetAllActiveMembershipsCountAsync();

            Assert.That(result, Is.EqualTo(2));
        }


        [Test]
        public async Task GetAllMembershipTypesShouldWorkProperly()
        {
            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = service.GetAllMembershipTypes();

            Assert.That(result.Count(), Is.EqualTo(3));
        }
    }
}

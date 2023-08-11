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
    using MyGymWorld.Web.ViewModels.Memberships;
    using MyGymWorld.Web.ViewModels.Memberships.Enums;
    using System.Runtime.CompilerServices;

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
                MembershipType = "Weekly"
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
                    MembershipType = MembershipType.Weekly,
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
                MembershipType = "Monthly"
            };

            await service.EditMembershipAsync(membershipId, editMembershipInputModel);

            var editedMembership = await this.dbContext.Memberships
                .FirstAsync(m => m.Id.ToString() == membershipId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(editedMembership.MembershipType.ToString(), Is.EqualTo("Monthly"));
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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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
        public async Task BuyMembershipAsyncShouldAddMembershipToUserWhenHeBuysItForTheFirstTime()
        {
            var membershipId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    GymId = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Weekly,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Monthly,
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
                .Setup(x => x.All<UserMembership>())
                .Returns(this.dbContext.UsersMemberships.AsQueryable());

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            this.qrCodeServiceMock
                .Setup(x => x.GenerateQRCodeAsync(membershipId))
                .ReturnsAsync(("testQrCodeUri", "testPublicId"));

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<UserMembership>()))
                .Callback(async (UserMembership userMembership) =>
                {
                    await this.dbContext.UsersMemberships.AddAsync(userMembership);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            await service.BuyMembershipAsync(membershipId, userId);

            var createdUserMembership = await this.dbContext.UsersMemberships
                .FirstAsync(um => um.MembershipId.ToString() == membershipId && um.UserId.ToString() == userId);

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<UserMembership>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(createdUserMembership);
            Assert.That(createdUserMembership.UserId.ToString(), Is.EqualTo(userId));
        }

        [Test]
        public async Task BuyMembershipAsyncShouldUpdateAlreadyBoughtMembership()
        {
            var membershipId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.Parse(membershipId),
                    GymId = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Weekly,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Monthly,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.UsersMemberships.AddAsync(new UserMembership
            {
                UserId = Guid.Parse(userId),
                MembershipId = Guid.Parse(membershipId),
                QRCodeUri = "Test",
                PublicId = "Test",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.All<UserMembership>())
                .Returns(this.dbContext.UsersMemberships.AsQueryable());

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Membership>())
                .Returns(this.dbContext.Memberships
                .Where(m => m.IsDeleted == false));

            this.qrCodeServiceMock
                .Setup(x => x.GenerateQRCodeAsync(membershipId))
                .ReturnsAsync(("testQrCodeUri", "testPublicId"));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            await service.BuyMembershipAsync(membershipId, userId);

            var updatedUserMembership = await this.dbContext.UsersMemberships
                .FirstAsync(um => um.MembershipId.ToString() == membershipId && um.UserId.ToString() == userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(updatedUserMembership);
            Assert.IsFalse(updatedUserMembership.IsDeleted);
        }

        [Test]
        public async Task GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsyncShouldWorkProperly()
        {
            var gymId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
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

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.Parse(gymId),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

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
                    UserId = Guid.Parse(userId),
                    MembershipId = Guid.Parse(membershipId),
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
                .Where(um => um.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            AllUserMemberhipsQueryModel allUserMemberhipsQueryModel = new AllUserMemberhipsQueryModel
            {
                UserId = userId,
                GymId = gymId,
                MembershipsPerPage = 2,
                Sorting = MembershipsSorting.Newest,
                CurrentPage = 1,
                TotalMembershipsCount = 2
            };

            var result = await service.GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsync(userId, allUserMemberhipsQueryModel);

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(MembershipViewModel));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("532fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsyncShouldReturnZeroWhenIdIsInvalid(string userId)
        {
            var gymId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var validUserId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.NewGuid(),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.Parse(validUserId),
                    MembershipId = Guid.Parse(membershipId),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.Parse(validUserId),
                    MembershipId = Guid.Parse(membershipId),
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
                .Where(um => um.IsDeleted == false));

            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            AllUserMemberhipsQueryModel allUserMemberhipsQueryModel = new AllUserMemberhipsQueryModel
            {
                UserId = userId,
                GymId = gymId,
                MembershipsPerPage = 2,
                Sorting = MembershipsSorting.Newest,
                CurrentPage = 1,
                TotalMembershipsCount = 2
            };

            var result = await service.GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsync(userId, allUserMemberhipsQueryModel);

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllActiveMembershipsFilteredAndPagedByGymIdAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

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
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Price = 10m,
                    MembershipType = MembershipType.Weekly,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Price = 10m,
                    MembershipType = MembershipType.Monthly,
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

            AllMembershipsForGymQueryModel allMembershipsForGymQueryModel = new AllMembershipsForGymQueryModel
            {
                GymId = gymId,
                MembershipsPerPage = 2,
                Sorting = MembershipsSorting.Newest,
                CurrentPage = 1,
                TotalMembershipsCount = 2
            };

            var result = await service.GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(allMembershipsForGymQueryModel);

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(MembershipViewModel));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllActiveMembershipsFilteredAndPagedByGymIdAsyncShouldReturnZeroWhenIdIsInvalid(string gymId)
        {
            var validGymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddAsync(new Gym
            {
                Id = Guid.Parse(validGymId),
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
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(validGymId),
                    Price = 10m,
                    MembershipType = MembershipType.Weekly,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(validGymId),
                    Price = 10m,
                    MembershipType = MembershipType.Monthly,
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

            AllMembershipsForGymQueryModel allMembershipsForGymQueryModel = new AllMembershipsForGymQueryModel
            {
                GymId = gymId,
                MembershipsPerPage = 2,
                Sorting = MembershipsSorting.Newest,
                CurrentPage = 1,
                TotalMembershipsCount = 2
            };

            var result = await service.GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(allMembershipsForGymQueryModel);

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetActivePaymentsByGymIdForManagementAsyncShouldWorkProperly()
        {
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                Email = "Test",
                UserName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.Parse(gymId),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

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
                    UserId = Guid.Parse(userId),
                    MembershipId = Guid.Parse(membershipId),
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

            var result = await service.GetActivePaymentsByGymIdForManagementAsync(gymId, 1, 1);

            Assert.That(result.Count(), Is.EqualTo(1));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PayedMembershipViewModel));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetActivePaymentsByGymIdForManagementAsyncShouldReturnZeroWhenIdIsInvalid(string gymId)
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "Test",
                UserName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.NewGuid(),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
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

            var result = await service.GetActivePaymentsByGymIdForManagementAsync(gymId, 1, 1);

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetActivePaymentsByUserIdAsyncShouldWorkProperly()
        {
            var userId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                Email = "Test",
                UserName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.NewGuid(),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

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
                    UserId = Guid.Parse(userId),
                    MembershipId = Guid.Parse(membershipId),
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

            var result = await service.GetActivePaymentsByUserIdAsync(userId, 1, 1);

            Assert.That(result.Count(), Is.EqualTo(1));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(PayedMembershipViewModel));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetActivePaymentsByUserIdAsyncShouldReturnZeroWhenIdIsInvalid(string userId)
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "Test",
                UserName = "Test",
                IsDeleted = false
            });

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.NewGuid(),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
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

            var result = await service.GetActivePaymentsByUserIdAsync(userId, 1, 1);

            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetActivePaymentsCountByGymIdAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.Parse(gymId),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
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

            var result = await service.GetActivePaymentsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetActivePaymentsCountByGymIdAsyncShouldReturnZeroWhenIdIsInvalid(string gymId)
        {
            var membershipId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                GymId = Guid.NewGuid(),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.NewGuid(),
                    MembershipId = Guid.Parse(membershipId),
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

            var result = await service.GetActivePaymentsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllActiveMembershipsCountByGymIdAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Price = 10m,
                    MembershipType = MembershipType.Weekly,
                    IsDeleted = false
                },
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                     Price = 10m,
                    MembershipType = MembershipType.Monthly,
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

            var result = await service.GetAllActiveMembershipsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllActiveMembershipsCountByGymIdAsyncShouldReturnZeroWhenIdIsInvalid(string gymId)
        {
            await this.dbContext.Memberships.AddRangeAsync(new HashSet<Membership>
            {
                new Membership
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    Price = 10m,
                    MembershipType = MembershipType.Weekly,
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

            var result = await service.GetAllActiveMembershipsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllActiveUserMembershipsCountByUserIdAsyncShouldWorkProperly()
        {
            var userId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersMemberships.AddRangeAsync(new HashSet<UserMembership>
            {
                new UserMembership
                {
                    UserId = Guid.Parse(userId),
                    MembershipId = Guid.NewGuid(),
                    QRCodeUri = "Test",
                    PublicId = "Test",
                    IsDeleted = false
                },
                new UserMembership
                {
                    UserId = Guid.Parse(userId),
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

            var result = await service.GetAllActiveUserMembershipsCountByUserIdAsync(userId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllActiveUserMembershipsCountByUserIdAsyncShouldReturnZeroWhenIdIsInvalid(string userId)
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

            var result = await service.GetAllActiveUserMembershipsCountByUserIdAsync(userId);

            Assert.That(result, Is.EqualTo(0));
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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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

            await this.dbContext.Memberships.AddAsync(new Membership
            {
                Id = Guid.Parse(membershipId),
                Price = 10m,
                MembershipType = MembershipType.Weekly,
                IsDeleted = false
            });

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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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
                    MembershipType = MembershipType.Weekly,
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
        public void GetAllMembershipTypesShouldWorkProperly()
        {
            var service = new MembershipService(this.mapper, this.mockRepository.Object, this.qrCodeServiceMock.Object);

            var result = service.GetAllMembershipTypes();

            Assert.That(result.Count(), Is.EqualTo(5));
        }
    }
}
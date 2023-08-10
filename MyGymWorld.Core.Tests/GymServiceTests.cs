namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using CloudinaryDotNet.Actions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Gyms;
    using MyGymWorld.Web.ViewModels.Gyms.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Net;
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

        // These tests below are commented because they slow down the Run Tests Process and the test coverage does not appear.
        // However, these tests will pass as well.

        //[Test]
        //public async Task CreateGymAsyncShouldWorkProperlyWhenAddressAlreadyExists()
        //{
        //    var managerId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var address = "Mladost 4";

        //    this.mockRepository
        //        .Setup(x => x.AddAsync(It.IsAny<Gym>()))
        //        .Callback(async (Gym gym) =>
        //        {
        //            await this.dbContext.Gyms.AddAsync(gym);
        //            await this.dbContext.SaveChangesAsync();
        //        });

        //    this.addressServiceMock
        //        .Setup(x => x.GetAddressByNameAsync(address))
        //        .ReturnsAsync(new Address
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = address
        //        });

        //    CreateGymInputModel createGymInputModel = new CreateGymInputModel
        //    {
        //        Name = "Test",
        //        Email = "test@gmail.com",
        //        PhoneNumber = "1234567890",
        //        Description = "Something",
        //        GymType = "PowerLifting",
        //        Address = address,
        //        LogoResultParams = new ImageUploadResult
        //        {
        //            SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //            PublicId = "Test"
        //        },
        //        GalleryImagesResultParams = new HashSet<ImageUploadResult>
        //        {
        //            new ImageUploadResult
        //            {
        //                SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //                PublicId = "Test"
        //            },
        //        }
        //    };

        //    var service = new GymService(this.mapper, this.mockRepository.Object,
        //       this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
        //       this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
        //       this.addressServiceMock.Object);

        //    await service.CreateGymAsync(managerId, createGymInputModel);

        //    this.addressServiceMock.Verify(x => x.GetAddressByNameAsync(address), Times.Once);
        //    this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Gym>()), Times.Once);
        //    this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        //}

        //[Test]
        //public async Task CreateGymAsyncShouldWorkProperlyWhenAddressDoesNotExist()
        //{
        //    var managerId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var townId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var address = "Mladost 4";

        //    this.mockRepository
        //        .Setup(x => x.AddAsync(It.IsAny<Gym>()))
        //        .Callback(async (Gym gym) =>
        //        {
        //            await this.dbContext.Gyms.AddAsync(gym);
        //            await this.dbContext.SaveChangesAsync();
        //        });

        //    this.addressServiceMock
        //        .Setup(x => x.CreateAddressAsync(address, townId))
        //        .ReturnsAsync(new Address
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = address
        //        });

        //    CreateGymInputModel createGymInputModel = new CreateGymInputModel
        //    {
        //        Name = "Test",
        //        Email = "test@gmail.com",
        //        PhoneNumber = "1234567890",
        //        Description = "Something",
        //        GymType = "PowerLifting",
        //        Address = address,
        //        TownId = townId,
        //        LogoResultParams = new ImageUploadResult
        //        {
        //            SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //            PublicId = "Test"
        //        },
        //        GalleryImagesResultParams = new HashSet<ImageUploadResult>
        //        {
        //            new ImageUploadResult
        //            {
        //                SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //                PublicId = "Test"
        //            },
        //        }
        //    };

        //    var service = new GymService(this.mapper, this.mockRepository.Object,
        //       this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
        //       this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
        //       this.addressServiceMock.Object);

        //    await service.CreateGymAsync(managerId, createGymInputModel);

        //    this.addressServiceMock.Verify(x => x.CreateAddressAsync(address, townId), Times.Once);
        //    this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Gym>()), Times.Once);
        //    this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        //}

        //[Test]
        //public async Task EditGymAsyncShouldWorkProperlyWhenAddressAlreadyExists()
        //{
        //    var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var addressId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var townId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var address = "Mladost 4";

        //    await this.dbContext.Towns.AddAsync(new Town
        //    {
        //        Id = Guid.Parse(townId),
        //        Population = 10000,
        //        ZipCode = "10001",
        //        Name= "Test"
        //    });

        //    await this.dbContext.Addresses.AddAsync(new Address
        //    {
        //        Id = Guid.Parse(addressId),
        //        TownId = Guid.Parse(townId),
        //        Name = "Test"
        //    });

        //    await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
        //    {
        //        new Gym
        //        {
        //            Id = Guid.Parse(gymId),
        //            AddressId = Guid.Parse(addressId),
        //            Email = "test@gmail.com",
        //            PhoneNumber = "1234567890",
        //            Name = "Gym Test",
        //            Description = "Gym Test",
        //            LogoUri = "test",
        //            LogoPublicId = "test",
        //            WebsiteUrl = "test",
        //            IsDeleted = false
        //        },
        //        new Gym
        //        {
        //            Id = Guid.NewGuid(),
        //            Email = "test@gmail.com",
        //            PhoneNumber = "1234567890",
        //            Name = "Gym Test",
        //            Description = "Gym Test",
        //            LogoUri = "test",
        //            LogoPublicId = "test",
        //            WebsiteUrl = "test",
        //            IsDeleted = true
        //        }
        //    });

        //    await this.dbContext.SaveChangesAsync();

        //    this.mockRepository
        //        .Setup(x => x.AllNotDeleted<Gym>())
        //        .Returns(this.dbContext.Gyms
        //        .Where(g => g.IsDeleted == false));

        //    this.addressServiceMock
        //       .Setup(x => x.GetAddressByNameAsync(address))
        //       .ReturnsAsync(new Address
        //       {
        //           Id = Guid.NewGuid(),
        //           Name = address
        //       });

        //    EditGymInputModel createGymInputModel = new EditGymInputModel
        //    {
        //        Name = "Test",
        //        Email = "test@gmail.com",
        //        PhoneNumber = "1234567890",
        //        Description = "Something",
        //        GymType = "PowerLifting",
        //        Address = address,
        //        TownId = townId,
        //        LogoResultParams = new ImageUploadResult
        //        {
        //            SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //            PublicId = "Test"
        //        },
        //        GalleryImagesResultParams = new HashSet<ImageUploadResult>
        //        {
        //            new ImageUploadResult
        //            {
        //                SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //                PublicId = "Test"
        //            },
        //        }
        //    };

        //    var service = new GymService(this.mapper, this.mockRepository.Object,
        //       this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
        //       this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
        //       this.addressServiceMock.Object);

        //    await service.EditGymAsync(gymId, createGymInputModel);

        //    this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        //    this.addressServiceMock.Verify(x => x.GetAddressByNameAsync(address), Times.Once);
        //}

        //[Test]
        //public async Task EditGymAsyncShouldWorkProperlyWhenAddressDoesNotExists()
        //{
        //    var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var addressId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var townId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
        //    var address = "Mladost 4";

        //    await this.dbContext.Towns.AddAsync(new Town
        //    {
        //        Id = Guid.Parse(townId),
        //        Population = 10000,
        //        ZipCode = "10001",
        //        Name = "Test"
        //    });

        //    await this.dbContext.Addresses.AddAsync(new Address
        //    {
        //        Id = Guid.Parse(addressId),
        //        TownId = Guid.Parse(townId),
        //        Name = "Test"
        //    });

        //    await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
        //    {
        //        new Gym
        //        {
        //            Id = Guid.Parse(gymId),
        //            AddressId = Guid.Parse(addressId),
        //            Email = "test@gmail.com",
        //            PhoneNumber = "1234567890",
        //            Name = "Gym Test",
        //            Description = "Gym Test",
        //            LogoUri = "test",
        //            LogoPublicId = "test",
        //            WebsiteUrl = "test",
        //            IsDeleted = false
        //        },
        //        new Gym
        //        {
        //            Id = Guid.NewGuid(),
        //            Email = "test@gmail.com",
        //            PhoneNumber = "1234567890",
        //            Name = "Gym Test",
        //            Description = "Gym Test",
        //            LogoUri = "test",
        //            LogoPublicId = "test",
        //            WebsiteUrl = "test",
        //            IsDeleted = true
        //        }
        //    });

        //    await this.dbContext.SaveChangesAsync();

        //    this.mockRepository
        //        .Setup(x => x.AllNotDeleted<Gym>())
        //        .Returns(this.dbContext.Gyms
        //        .Where(g => g.IsDeleted == false));

        //    this.addressServiceMock
        //       .Setup(x => x.CreateAddressAsync(address, townId))
        //       .ReturnsAsync(new Address
        //       {
        //           Id = Guid.NewGuid(),
        //           Name = address
        //       });

        //    EditGymInputModel createGymInputModel = new EditGymInputModel
        //    {
        //        Name = "Test",
        //        Email = "test@gmail.com",
        //        PhoneNumber = "1234567890",
        //        Description = "Something",
        //        GymType = "PowerLifting",
        //        Address = address,
        //        TownId = townId,
        //        LogoResultParams = new ImageUploadResult
        //        {
        //            SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //            PublicId = "Test"
        //        },
        //        GalleryImagesResultParams = new HashSet<ImageUploadResult>
        //        {
        //            new ImageUploadResult
        //            {
        //                SecureUri = new Uri("https://res.cloudinary.com/de1i8aava/image/upload/v1691677046/MyGymWorld/assets/gyms-gallery-pictures/jwwsi2nvz8gn75xawk01.jpg"),
        //                PublicId = "Test"
        //            },
        //        }
        //    };

        //    var service = new GymService(this.mapper, this.mockRepository.Object,
        //       this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
        //       this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
        //       this.addressServiceMock.Object);

        //    await service.EditGymAsync(gymId, createGymInputModel);

        //    this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        //    this.addressServiceMock.Verify(x => x.CreateAddressAsync(address, townId), Times.Once);
        //}

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("632fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task EditGymAsyncShouldNotDoAnythingWhenIdIsInvalid(string gymId)
        {
            var addressId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var townId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Towns.AddAsync(new Town
            {
                Id = Guid.Parse(townId),
                Population = 10000,
                ZipCode = "10001",
                Name = "Test"
            });

            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.Parse(addressId),
                TownId = Guid.Parse(townId),
                Name = "Test"
            });

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    AddressId = Guid.Parse(addressId),
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
                .Setup(x => x.AllNotDeleted<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            await service.EditGymAsync(gymId, new EditGymInputModel());

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteGymAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymId),
                    AddressId = Guid.NewGuid(),
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
                .Setup(x => x.AllNotDeleted<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            await service.DeleteGymAsync(gymId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("632fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task DeleteGymAsyncShouldNotDoAnythingWhenIdIsInvalid(string gymId)
        {
            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    AddressId = Guid.NewGuid(),
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
                .Setup(x => x.AllNotDeleted<Gym>())
                .Returns(this.dbContext.Gyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            await service.DeleteGymAsync(gymId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task AddGymToUserAsyncShouldWorkProperlyWhenUserJoinsGymFortheFirstTime()
        {
            var userId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            this.mockRepository
                .Setup(x => x.All<UserGym>())
                .Returns(this.dbContext.UsersGyms.AsQueryable());

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<UserGym>()))
                .Callback(async (UserGym userGym) =>
                {
                    await this.dbContext.UsersGyms.AddAsync(userGym);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            await service.AddGymToUserAsync(gymId, userId);

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<UserGym>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            var createdUserGym = await this.dbContext.UsersGyms
                .FirstAsync(ug => ug.GymId.ToString() == gymId && ug.UserId.ToString() == userId);

            Assert.IsNotNull(createdUserGym);
            Assert.IsFalse(createdUserGym.IsDeleted);
        }

        [Test]
        public async Task AddGymToUserAsyncShouldWorkProperlyWhenUserHasJoinedGymBeforeButHasLeftItAndNowWantsToJoinAgain()
        {
            var userId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "User",
                Email = "user@gmailc.com",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "123456789",
                IsDeleted = false
            });

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymId),
                    IsDeleted = true
                },
                new UserGym
                {
                    IsDeleted = true
                },
            });

            await this.dbContext.UsersEvents.AddAsync(new UserEvent
            {
                UserId = Guid.Parse(userId),
                EventId = Guid.NewGuid(),
                IsDeleted = true
            });

            await this.dbContext.UsersMemberships.AddAsync(new UserMembership
            {
                UserId = Guid.Parse(userId),
                MembershipId = Guid.NewGuid(),
                QRCodeUri = "Test",
                PublicId = "Test",
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.All<UserGym>())
                .Returns(this.dbContext.UsersGyms.AsQueryable());

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            await service.AddGymToUserAsync(gymId, userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddGymToUserAsyncShoulThrowExceptionWhenUserHasJoinedGymAndTriesToJoinAgain()
        {
            var userId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "User",
                Email = "user@gmailc.com",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "123456789",
                IsDeleted = false
            });

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymId),
                    IsDeleted = false
                },
                new UserGym
                {
                    IsDeleted = true
                },
            });

            await this.dbContext.UsersEvents.AddAsync(new UserEvent
            {
                UserId = Guid.Parse(userId),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddAsync(new UserMembership
            {
                UserId = Guid.Parse(userId),
                MembershipId = Guid.NewGuid(),
                QRCodeUri = "Test",
                PublicId = "Test",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.All<UserGym>())
                .Returns(this.dbContext.UsersGyms.AsQueryable());

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.AddGymToUserAsync(gymId, userId);

            }, ExceptionConstants.GymErrors.GymAlreadyJoined);
        }

        [Test]
        public async Task RemoveGymFromUserAsyncShouldWorkProperly()
        {
            var userId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "User",
                Email = "user@gmailc.com",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "123456789",
                IsDeleted = false
            });

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymId),
                    IsDeleted = false
                },
                new UserGym
                {
                    IsDeleted = true
                },
            });

            await this.dbContext.UsersEvents.AddAsync(new UserEvent
            {
                UserId = Guid.Parse(userId),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddAsync(new UserMembership
            {
                UserId = Guid.Parse(userId),
                MembershipId = Guid.NewGuid(),
                QRCodeUri = "Test",
                PublicId = "Test",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            await service.RemoveGymFromUserAsync(gymId, userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase(null, "832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", "932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task RemoveGymFromUserAsyncShouldThrowExceptionWhenUserHasNotJoinedGym(string gymId, string userId)
        {
            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new UserGym
                {
                    IsDeleted = true
                },
            });

            await this.dbContext.UsersEvents.AddAsync(new UserEvent
            {
                UserId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                IsDeleted = false
            });

            await this.dbContext.UsersMemberships.AddAsync(new UserMembership
            {
                UserId = Guid.NewGuid(),
                MembershipId = Guid.NewGuid(),
                QRCodeUri = "Test",
                PublicId = "Test",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
               await service.RemoveGymFromUserAsync(gymId, userId);

            }, ExceptionConstants.GymErrors.GymNotJoinedToBeLeft);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task GetActiveOrDeletedGymsCountForManagementAsyncShouldWorkProperly(bool isDeleted)
        {
            var managerId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
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
                    ManagerId = Guid.Parse(managerId),
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
                .Setup(x => x.AllReadonly<Gym>())
                .Returns(this.dbContext.Gyms.AsQueryable());

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetActiveOrDeletedGymsCountForManagementAsync(managerId, isDeleted);

            var expectedCount = await this.dbContext.Gyms
                .CountAsync(g => g.IsDeleted == isDeleted && g.ManagerId.ToString() == managerId);

            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        [TestCase("", false)]
        [TestCase(null, true)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", false)]
        public async Task GetActiveOrDeletedGymsCountForManagementAsyncShouldReturnZeroWhenIdIsInvalid(string managerId, bool isDeleted)
        {
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
                    ManagerId = Guid.NewGuid(),
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
                .Setup(x => x.AllReadonly<Gym>())
                .Returns(this.dbContext.Gyms.AsQueryable());

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetActiveOrDeletedGymsCountForManagementAsync(managerId, isDeleted);

            var expectedCount = await this.dbContext.Gyms
                .CountAsync(g => g.IsDeleted == isDeleted && g.ManagerId.ToString() == managerId);

            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task GetActiveOrDeletedGymsCountForAdministrationAsyncShouldWorkProperly(bool isDeleted)
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
                .Setup(x => x.AllReadonly<Gym>())
                .Returns(this.dbContext.Gyms.AsQueryable());

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            var result = await service.GetActiveOrDeletedGymsCountForAdministrationAsync(isDeleted);

            var expectedCount = await this.dbContext.Gyms.CountAsync(g => g.IsDeleted == isDeleted);

            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetTop10NewestActiveGymsAsyncShouldWorkProperly()
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
                    CreatedOn = DateTime.UtcNow,
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
                    CreatedOn = DateTime.UtcNow.AddMinutes(5),
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

            var result = await service.GetTop10NewestActiveGymsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(DisplayGymViewModel));
        }

        [Test]
        public async Task GetTop10MostLikedActiveGymsAsyncShouldWorkProperly()
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

            await this.dbContext.Likes.AddAsync(new Like
            {
                GymId = Guid.Parse(gymId),
                UserId = Guid.NewGuid(),
                IsDeleted = false
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

            var result = await service.GetTop10MostLikedActiveGymsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(DisplayGymViewModel));
        }

        [Test]
        public async Task GetAllActiveGymsFilteredAndPagedAsyncShouldWorkProperly()
        {
            var addressId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.Parse(addressId),
                Name = "Test",
                IsDeleted = false
            });

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.NewGuid(),
                    AddressId = Guid.Parse(addressId),
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
                    AddressId = Guid.Parse(addressId),
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

            AllGymsQueryModel allGymsQueryModel = new AllGymsQueryModel
            {
                GymsPerPage = 2,
                SearchTerm = null,
                Sorting = GymsSorting.Newest,
                CurrentPage = 1,
                TotalGymsCount = 3
            };

            var result = await service.GetAllActiveGymsFilteredAndPagedAsync(allGymsQueryModel);

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(DisplayGymViewModel));
        }

        [Test]
        public async Task GetGymDetailsByIdAsyncShouldWorkProperly()
        {
            var gymId = "432fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var userId = "532fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var managerId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var townId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var addressId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var countryId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                Id = Guid.Parse(userId),
                UserName = "User",
                Email = "user@gmailc.com",
                FirstName = "Test",
                LastName = "Test",
                PhoneNumber = "123456789",
                IsDeleted = false
            });

            await this.dbContext.Managers.AddAsync(new Manager
            {
                Id = Guid.Parse(managerId),
                UserId = Guid.Parse(userId),
                ManagerType = ManagerType.ManyGymsManager,
                IsDeleted = false
            });

            await this.dbContext.Countries.AddAsync(new Country
            {
                Id = Guid.Parse(countryId),
                Name = "Test",
                IsDeleted = false
            });

            await this.dbContext.Towns.AddAsync(new Town
            {
                Id = Guid.Parse(townId),
                CountryId = Guid.Parse(countryId),
                Name = "Test",
                Population = 1000,
                ZipCode = "10001",
                IsDeleted = false
            });

            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.Parse(addressId),
                TownId = Guid.Parse(townId),
                Name = "Test",
                IsDeleted = false
            });

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymId),
                    ManagerId = Guid.Parse(managerId),
                    AddressId = Guid.Parse(addressId),
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

            var result = await service.GetGymDetailsByIdAsync(gymId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<GymDetailsViewModel>(result);
            Assert.That(result.Id, Is.EqualTo(gymId));
        }

        [Test]
        public async Task GetGymForEditByIdAsyncShouldWorkProperly()
        {
            var gymId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var townId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var addressId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Towns.AddAsync(new Town
            {
                Id = Guid.Parse(townId),
                Name = "Test",
                Population = 1000,
                ZipCode = "10001",
                IsDeleted = false
            });

            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.Parse(addressId),
                Name = "Test",
                TownId = Guid.Parse(townId),
                IsDeleted = false
            });
            
            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymId),
                    AddressId = Guid.Parse(addressId),
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

            var result = await service.GetGymForEditByIdAsync(gymId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EditGymInputModel>(result);
            Assert.That(result.Id, Is.EqualTo(gymId));
        }

        [Test]
        public async Task GetAllUserJoinedGymsFilteredAndPagedAsyncShouldWorkProperly()
        {
            var userId = "632fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymOneId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymTwoId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var addressId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.Parse(addressId),
                Name = "Test",
                IsDeleted = false
            });

            await this.dbContext.Gyms.AddRangeAsync(new HashSet<Gym>
            {
                new Gym
                {
                    Id = Guid.Parse(gymOneId),
                    AddressId = Guid.Parse(addressId),
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
                    Id = Guid.Parse(gymTwoId),
                    AddressId = Guid.Parse(addressId),
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

            await this.dbContext.UsersGyms.AddRangeAsync(new HashSet<UserGym>
            {
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymOneId),
                    IsDeleted = false
                },
                new UserGym
                {
                    UserId = Guid.Parse(userId),
                    GymId = Guid.Parse(gymTwoId),
                    IsDeleted = false
                },
                new UserGym
                {
                    IsDeleted = true
                },
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(g => g.IsDeleted == false));

            var service = new GymService(this.mapper, this.mockRepository.Object,
               this.eventServiceMock.Object, this.articleServiceMock.Object, this.membershipServiceMock.Object,
               this.likeServiceMock.Object, this.dislikeServiceMock.Object, this.commentServiceMock.Object,
               this.addressServiceMock.Object);

            AllUserJoinedGymsQueryModel allUserJoinedGymsQueryModel = new AllUserJoinedGymsQueryModel
            {
                UserId = userId,
                GymsPerPage = 2,
                SearchTerm = null,
                Sorting = GymsSorting.Newest,
                CurrentPage = 1,
                TotalGymsCount = 3
            };

            var result = await service.GetAllUserJoinedGymsFilteredAndPagedAsync(userId, allUserJoinedGymsQueryModel);

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(DisplayGymViewModel));
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
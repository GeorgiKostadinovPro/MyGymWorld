namespace MyGymWorld.Core.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System;
    using System.Security.Cryptography.X509Certificates;

    [TestFixture]
    public class LikeServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private Mock<IRepository> mockRepository;

        [SetUp]
        public async Task Setup()
        {
            this.mockRepository = new Mock<IRepository>();

            DbContextOptions<MyGymWorldDbContext> _options = new DbContextOptionsBuilder<MyGymWorldDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDb")
                      .Options;

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateLikeAsyncShouldWorkProperlyWhenLikeDoesNOTExist()
        {
            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.All<Like>())
                .Returns(this.dbContext.Likes.AsQueryable());

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Dislike>())
                .Returns(this.dbContext.Dislikes
                .Where(dl => dl.IsDeleted == false)
                .AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            var result = await service.CreateLikeAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            this.mockRepository.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.That(result.IsDeleted, Is.False);
        }

        [Test]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", false)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", true)]
        public async Task CreateLikeAsyncShouldWorkProperlyWhenLikeExistsAndDislikeExist(string gymId, string userId, bool isDeleted)
        {
            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    GymId = Guid.Parse(gymId), 
                    UserId = Guid.Parse(userId),
                    IsDeleted = isDeleted
                },
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    GymId = Guid.Parse(gymId),
                    UserId = Guid.Parse(userId),
                    IsDeleted = false
                },
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.All<Like>())
                .Returns(this.dbContext.Likes.AsQueryable());

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Dislike>())
                .Returns(this.dbContext.Dislikes
                .Where(dl => dl.IsDeleted == false)
                .AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            var result = await service.CreateLikeAsync(gymId, userId);

            this.mockRepository.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            var dislikeChanged = await this.dbContext.Dislikes
                .FirstAsync(dl => dl.UserId.ToString() == userId);

            Assert.IsNotNull(result);
            Assert.That(result.IsDeleted, Is.EqualTo(!isDeleted));

            Assert.That(dislikeChanged.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteLikeAsyncShouldWorkProperly()
        {
            var likeId = Guid.NewGuid();

            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    Id = likeId,
                    IsDeleted = false
                },
                new Like
                {
                    Id =  Guid.NewGuid(),
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Like>())
                .Returns(this.dbContext.Likes
                .Where(l => l.IsDeleted == false)
                .AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            await service.DeleteLikeAsync(likeId.ToString());

            var deletedLike = await this.dbContext.Likes.FirstAsync(x => x.Id == likeId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(deletedLike.IsDeleted, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]

        public async Task DeleteLikeAsyncShouldNotDoAnythingWhenIdIsInvalid(string likeId)
        {
            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Like
                {
                    Id =  Guid.NewGuid(),
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Like>())
                .Returns(this.dbContext.Likes
                .Where(l => l.IsDeleted == false)
                .AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            await service.DeleteLikeAsync(likeId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);

            int count = await this.dbContext.Likes.CountAsync(l => l.IsDeleted == false);

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllActiveLikesCountAsyncShouldWorkProperly()
        {
            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    IsDeleted = true
                },
                new Like
                {
                    IsDeleted = false
                },
                new Like
                {
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Like>())
                .Returns(this.dbContext.Likes
                .Where(l => l.IsDeleted == false)
                .AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            var result = await service.GetAllActiveLikesCountAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task CheckIfUserLikedGymAsyncShouldWorkProperly()
        {
            var userId = Guid.NewGuid();
            var gymId = Guid.NewGuid();

            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    UserId = userId,
                    GymId = gymId,
                    IsDeleted = false
                },
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<Like>())
                .Returns(this.dbContext.Likes.AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            var result = await service.CheckIfUserLikedGymAsync(gymId.ToString(), userId.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("", "932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "")]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfUserLikedGymAsyncShouldReturnFalsewWhenDataIsInvalid(string gymId, string userId)
        {
            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Like
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Like
                {
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    GymId = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<Like>())
                .Returns(this.dbContext.Likes.AsQueryable());

            var service = new LikeService(this.mockRepository.Object);

            var result = await service.CheckIfUserLikedGymAsync(gymId, null);

            Assert.IsFalse(result);
        }
    }
}

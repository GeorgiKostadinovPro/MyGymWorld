namespace MyGymWorld.Core.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Data.Models;

    [TestFixture]
    public class DislikeServiceTests
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
        public async Task CreateDislikeAsyncShouldWorkProperlyWhenLikeDoesNOTExist()
        {
            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.All<Dislike>())
                .Returns(this.dbContext.Dislikes.AsQueryable());

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Like>())
                .Returns(this.dbContext.Likes
                .Where(dl => dl.IsDeleted == false)
                .AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            var result = await service.CreateDislikeAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            this.mockRepository.Verify(r => r.AddAsync(It.IsAny<Dislike>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(result);
            Assert.That(result.IsDeleted, Is.False);
        }

        [Test]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", false)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", true)]
        public async Task CreateDislikeAsyncShouldWorkProperlyWhenLikeExistsAndDislikeExist(string gymId, string userId, bool isDeleted)
        {
            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    GymId = Guid.Parse(gymId),
                    UserId = Guid.Parse(userId),
                    IsDeleted = isDeleted
                },
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = true
                }
            });

            await this.dbContext.Likes.AddRangeAsync(new HashSet<Like>
            {
                new Like
                {
                    GymId = Guid.Parse(gymId),
                    UserId = Guid.Parse(userId),
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
                .Setup(x => x.All<Dislike>())
                .Returns(this.dbContext.Dislikes.AsQueryable());

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Like>())
                .Returns(this.dbContext.Likes
                .Where(dl => dl.IsDeleted == false)
                .AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            var result = await service.CreateDislikeAsync(gymId, userId);

            this.mockRepository.Verify(r => r.AddAsync(It.IsAny<Dislike>()), Times.Never);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            var likeChanged = await this.dbContext.Likes
                .FirstAsync(dl => dl.UserId.ToString() == userId);

            Assert.IsNotNull(result);
            Assert.That(result.IsDeleted, Is.EqualTo(!isDeleted));

            Assert.That(likeChanged.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteDislikeAsyncShouldWorkProperly()
        {
            var dislikeId = Guid.NewGuid();

            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    Id = dislikeId,
                    IsDeleted = false
                },
                new Dislike
                {
                    Id =  Guid.NewGuid(),
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Dislike>())
                .Returns(this.dbContext.Dislikes
                .Where(l => l.IsDeleted == false)
                .AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            await service.DeleteDislikeAsync(dislikeId.ToString());

            var deletedDislike = await this.dbContext.Dislikes.FirstAsync(x => x.Id == dislikeId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(deletedDislike.IsDeleted, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]

        public async Task DeleteDislikeAsyncShouldNotDoAnythingWhenIdIsInvalid(string likeId)
        {
            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Dislike
                {
                    Id =  Guid.NewGuid(),
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Dislike>())
                .Returns(this.dbContext.Dislikes
                .Where(l => l.IsDeleted == false)
                .AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            await service.DeleteDislikeAsync(likeId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);

            int count = await this.dbContext.Dislikes.CountAsync(l => l.IsDeleted == false);

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task CheckIfUserDislikedGymAsyncShouldWorkProperly()
        {
            var userId = Guid.NewGuid();
            var gymId = Guid.NewGuid();

            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    UserId = userId,
                    GymId = gymId,
                    IsDeleted = false
                },
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
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
                .Setup(x => x.AllReadonly<Dislike>())
                .Returns(this.dbContext.Dislikes.AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            var result = await service.CheckIfUserDislikedGymAsync(gymId.ToString(), userId.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("", "932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "")]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e", "832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfUserDislikedGymAsyncShouldReturnFalsewWhenDataIsInvalid(string gymId, string userId)
        {
            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Dislike
                {
                    UserId = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new Dislike
                {
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    GymId = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllReadonly<Dislike>())
                .Returns(this.dbContext.Dislikes.AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            var result = await service.CheckIfUserDislikedGymAsync(gymId, userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllActiveDislikesCountAsyncShouldWorkProperly()
        {
            await this.dbContext.Dislikes.AddRangeAsync(new HashSet<Dislike>
            {
                new Dislike
                {
                    IsDeleted = true
                },
                new Dislike
                {
                    IsDeleted = false
                },
                new Dislike
                {
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Dislike>())
                .Returns(this.dbContext.Dislikes
                .Where(l => l.IsDeleted == false)
                .AsQueryable());

            var service = new DislikeService(this.mockRepository.Object);

            var result = await service.GetAllActiveDislikesCountAsync();

            Assert.That(result, Is.EqualTo(2));
        }
    }
}

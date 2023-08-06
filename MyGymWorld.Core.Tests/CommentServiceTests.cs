namespace MyGymWorld.Core.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Web.ViewModels.Comments;

    [TestFixture]
    public class CommentServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private Mock<IRepository> mockRepository;
        private IMapper mapper; 

        [SetUp]
        public async Task Setup()
        {
            this.mockRepository = new Mock<IRepository>();

            this.mapper = InitializeAutoMapper.CreateMapper();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateCommentAsyncWithoutParentIdShouldWorkProperly()
        {
            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Callback(async (Comment comment) =>
                {
                    await this.dbContext.Comments.AddAsync(comment);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            await service.CreateCommentAsync(
                "932fe39a-bc5b-4ea4-b0c5-68b2da06768e",
                "832fe39a-bc5b-4ea4-b0c5-68b2da06768e",
                "Test comment",
                null);

            var createdComment = await this.dbContext.Comments.FirstOrDefaultAsync(
                c => c.GymId == Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")
                && c.UserId == Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"));

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Comment>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(createdComment);
            Assert.That(createdComment.IsDeleted, Is.False);
        }


        [Test]
        public async Task CreateCommentWithParentIdAsyncShouldWorkProperly()
        {
            await this.dbContext.Comments.AddAsync(new Comment
            {
                Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                Content = "Test comment"
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Callback(async (Comment comment) =>
                {
                    await this.dbContext.Comments.AddAsync(comment);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            await service.CreateCommentAsync(
                "932fe39a-bc5b-4ea4-b0c5-68b2da06768e",
                "832fe39a-bc5b-4ea4-b0c5-68b2da06768e",
                "Test comment",
                "732fe39a-bc5b-4ea4-b0c5-68b2da06768e");

            var createdComment = await this.dbContext.Comments.FirstOrDefaultAsync(
                c => c.GymId == Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")
                && c.UserId == Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"));

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<Comment>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsNotNull(createdComment);
            Assert.That(createdComment.ParentId, Is.Not.Null);
            Assert.That(createdComment.IsDeleted, Is.False);
        }

        [Test]
        public async Task DeleteCommentAsyncShouldWorkProperly()
        {
            await this.dbContext.Comments.AddAsync(new Comment
            {
                Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                Content = "Test comment",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Comment>())
                .Returns(this.dbContext.Comments
                .Where(c => c.IsDeleted == false)
                .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            await service.DeleteCommentAsync("732fe39a-bc5b-4ea4-b0c5-68b2da06768e");

            var deletedComment = await this.dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"));

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(deletedComment!.IsDeleted, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task DeleteCommentAsyncShouldNotDoAnythingWhenIdIsInvalid(string commentId)
        {
            await this.dbContext.Comments.AddAsync(new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Test comment",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Comment>())
                .Returns(this.dbContext.Comments
                .Where(c => c.IsDeleted == false)
                .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            await service.DeleteCommentAsync(commentId);

            var count = await this.dbContext.Comments.CountAsync(c => c.IsDeleted == true);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", 0, 2)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", 1, 1)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e", 0, null)]

        public async Task GetActiveCommentsByGymIdShouldWorkProperly(string gymId, int skip, int? take = null)
        {
            ApplicationUser user = new ApplicationUser
            {
                Id = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                UserName = "Georgi",
                Email = "test@gmail.com"
            };

            await this.dbContext.Users.AddAsync(user);

            await this.dbContext.SaveChangesAsync();

            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    GymId = Guid.Parse(gymId),
                    Content = "Test comment",
                    CreatedOn = DateTime.UtcNow,
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    User = user,
                    IsDeleted = false,
                },
                new Comment
                {
                    GymId = Guid.Parse(gymId),
                    Content = "Test comment",
                    CreatedOn = DateTime.UtcNow.AddMinutes(5),
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    User = user,
                    IsDeleted = false,
                },
                 new Comment
                {
                    GymId = Guid.Parse(gymId),
                    Content = "Test comment",
                    CreatedOn = DateTime.UtcNow.AddMinutes(10),
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    User = user,
                    IsDeleted = false
                },
                new Comment
                {
                    GymId = Guid.Parse(gymId),
                    Content = "Test comment",
                    CreatedOn = DateTime.UtcNow.AddMinutes(6),
                    UserId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    User = user,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service
                .GetActiveCommentsByGymIdAsync(
                gymId, skip, take);

            int count = take.HasValue ? take.Value : (await this.dbContext.Comments
                .Where(c => c.IsDeleted == false && c.GymId.ToString() == gymId).CountAsync());

            Assert.That(result.Count(), Is.EqualTo(count));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(CommentViewModel));
        }

        [Test]
        public async Task GetAllActiveCommentsAsyncShouldWorkProperly()
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Comment>())
                .Returns(this.dbContext.Comments
                .Where(c => c.IsDeleted == false)
                .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveCommentsCountAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetCommentByIdAsyncShouldWorkProperly()
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service.GetComentByIdAsync("732fe39a-bc5b-4ea4-b0c5-68b2da06768e");

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]

        public async Task GetCommentByIdAsyncShouldReturnNullWhenIdIsInvalid(string commentId)
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service.GetComentByIdAsync(commentId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetActiveCommentsCountByGymIdAsyncShouldWorkProperly()
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    GymId = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                { 
                    GymId = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service.GetActiveCommentsCountByGymIdAsync("832fe39a-bc5b-4ea4-b0c5-68b2da06768e");

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetActiveCommentsCountByGymIdAsyncShouldReturnZeroWhenGymIdIsInvalid(string gymId)
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    GymId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    GymId = Guid.Parse("932fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service.GetActiveCommentsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task IsInSameGymAsyncShouldWorkProperly()
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    GymId = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    GymId = Guid.Parse("832fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service
                .IsInSameGymByIdAsync(
                "732fe39a-bc5b-4ea4-b0c5-68b2da06768e", 
                "832fe39a-bc5b-4ea4-b0c5-68b2da06768e");

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("832fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task IsInSameGymAsyncShouldReturnFalseWhenGymIdIsInvalid(string gymId)
        {
            await this.dbContext.Comments.AddRangeAsync(new HashSet<Comment>
            {
                new Comment
                {
                    Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    GymId = Guid.NewGuid(),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    GymId = Guid.NewGuid(),
                    Content = "Test comment",
                    IsDeleted = false
                },
                new Comment
                {
                    Content = "Test comment",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Comment>())
               .Returns(this.dbContext.Comments
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CommentService(this.mapper, this.mockRepository.Object);

            var result = await service
                .IsInSameGymByIdAsync(
                "732fe39a-bc5b-4ea4-b0c5-68b2da06768e",
                gymId);

            Assert.That(result, Is.False);
        }
    }
}

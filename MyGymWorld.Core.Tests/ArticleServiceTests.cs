namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Articles;
    using MyGymWorld.Web.ViewModels.Articles.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Articles;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO.IsolatedStorage;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    [TestFixture]
    public class ArticleServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;

        [SetUp]
        public async Task Setup()
        {
            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateArticleAsyncShouldWorkProperly()
        {
            string gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            string categoryId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddAsync(new Gym
            {
                Id = Guid.Parse(gymId),
                Name = "Test",
                PhoneNumber = "Test",
                Description = "Test",
                Email = "Test",
                LogoUri = "test",
                LogoPublicId = "test",
                WebsiteUrl = "test",
                IsDeleted = false
            });

            await this.dbContext.Categories.AddAsync(new Category
            {
                Id = Guid.Parse(categoryId),
                Name = "Test category",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Article>()))
                .Callback(async (Article article) =>
                {
                    await this.dbContext.Articles.AddAsync(article);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            CreateArticleInputModel createArticleInputModel = new CreateArticleInputModel
            {
                GymId = gymId,
                Title = "Test",
                Content = "Test content",
                CategoryIds = new string[1] { categoryId }
            };

            var result = await service.CreateArticleAsync(createArticleInputModel);

            var count = await this.dbContext.Articles
                .CountAsync(a => a.IsDeleted == false);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Article>(result);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task EditArticleAsyncShouldWorkProperly()
        {
            var articleId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var catgeoryOneId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var catgeoryTwoId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddAsync(new Article
            {
                Id = Guid.Parse(articleId),
                Title = "Test",
                Content = "Test",
                IsDeleted = false
            });

            await this.dbContext.Categories.AddRangeAsync(new HashSet<Category>
            {
                new Category
                {
                    Id = Guid.Parse(catgeoryOneId),
                    Name = "Test1",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse(catgeoryTwoId),
                    Name = "Test2",
                    IsDeleted = false
                }
            });

            await this.dbContext.ArticlesCategories.AddAsync(new ArticleCategory
            {
                ArticleId = Guid.Parse(articleId),
                CategoryId = Guid.Parse(catgeoryOneId),
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            this.mockRepository
                .Setup(x => x.All<ArticleCategory>())
                .Returns(this.dbContext.ArticlesCategories.AsQueryable());

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            EditArticleInputModel editArticleInputModel = new EditArticleInputModel 
            {
                Title = "New Test",
                Content = "New Content",
                GymId = Guid.NewGuid().ToString(),
                CategoryIds = new string[2] { catgeoryOneId, catgeoryTwoId }
            };

            await service.EditArticleAsync(articleId, editArticleInputModel);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }


        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task EditArticleAsyncShouldNotDoAnythingWhenArticleIdIsInvalid(string articleId)
        {
            var validArticleId = Guid.NewGuid().ToString();
            var catgeoryOneId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var catgeoryTwoId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddAsync(new Article
            {
                Id = Guid.Parse(validArticleId),
                Title = "Test",
                Content = "Test",
                IsDeleted = false
            });

            await this.dbContext.Categories.AddRangeAsync(new HashSet<Category>
            {
                new Category
                {
                    Id = Guid.Parse(catgeoryOneId),
                    Name = "Test1",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.Parse(catgeoryTwoId),
                    Name = "Test2",
                    IsDeleted = false
                }
            });

            await this.dbContext.ArticlesCategories.AddAsync(new ArticleCategory
            {
                ArticleId = Guid.Parse(validArticleId),
                CategoryId = Guid.Parse(catgeoryOneId),
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            this.mockRepository
                .Setup(x => x.All<ArticleCategory>())
                .Returns(this.dbContext.ArticlesCategories.AsQueryable());

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            EditArticleInputModel editArticleInputModel = new EditArticleInputModel
            {
                Title = "New Test",
                Content = "New Content",
                GymId = Guid.NewGuid().ToString(),
                CategoryIds = new string[2] { catgeoryOneId, catgeoryTwoId }
            };

            await service.EditArticleAsync(articleId, editArticleInputModel);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteArticleAsyncShouldWorkProperly()
        {
            string articleId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            string categoryId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddAsync(new Article 
            {
                Id = Guid.Parse(articleId),
                Title = "Test",
                Content = "Test",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            await service.DeleteArticleAsync(articleId);

            var count = await this.dbContext.Articles
                .CountAsync(a => a.IsDeleted == false);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]

        public async Task DeleteArticleAsyncShouldNotDoAnythingWhenArticleIdIsInvalid(string articleId)
        {
            await this.dbContext.Articles.AddAsync(new Article
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Content = "Test",
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            await service.DeleteArticleAsync(articleId);

            var count = await this.dbContext.Articles
                .CountAsync(a => a.IsDeleted == false);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SubscribeUserToGymArticlesAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersGyms.AddAsync(new UserGym
            {
                UserId = Guid.Parse(userId),
                GymId = Guid.Parse(gymId),
                IsSubscribedForArticles = false,
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new ArticleService(
                this.mapper,
                this.mockRepository.Object);

            await service.SubscribeUserToGymArticlesAsync(userId, gymId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase(
            null, 
            "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", false)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e", 
            null, false)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e", 
            "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", true)]
        public async Task SubscribeUserToGymArticlesAsyncShouldNotDoAnythingWhenParamsAreInvalid(string userId, string gymId, bool isSubscribed)
        {
            var validUserId = userId != null ? userId : Guid.NewGuid().ToString();
            var validGymId = gymId != null ? gymId : Guid.NewGuid().ToString();

            await this.dbContext.UsersGyms.AddAsync(new UserGym
            {
                UserId = Guid.Parse(validUserId),
                GymId = Guid.Parse(validGymId),
                IsSubscribedForArticles = isSubscribed,
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new ArticleService(
                this.mapper,
                this.mockRepository.Object);

            await service.SubscribeUserToGymArticlesAsync(userId, gymId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task UnsubscribeUserToGymArticlesAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersGyms.AddAsync(new UserGym
            {
                UserId = Guid.Parse(userId),
                GymId = Guid.Parse(gymId),
                IsSubscribedForArticles = true,
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new ArticleService(
                this.mapper,
                this.mockRepository.Object);

            await service.UnsubscribeUserToGymArticlesAsync(userId, gymId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase(
            null,
            "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", true)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e",
            null, true)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e",
            "832fe39a-bc5b-4ea4-b0c5-68b2da06768e", false)]
        public async Task UnsubscribeUserToGymArticlesAsyncShouldNotDoAnythingWhenParamsAreInvalid(string userId, string gymId, bool isSubscribed)
        {
            var validUserId = userId != null ? userId : Guid.NewGuid().ToString();
            var validGymId = gymId != null ? gymId : Guid.NewGuid().ToString();

            await this.dbContext.UsersGyms.AddAsync(new UserGym
            {
                UserId = Guid.Parse(validUserId),
                GymId = Guid.Parse(validGymId),
                IsSubscribedForArticles = isSubscribed,
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<UserGym>())
                .Returns(this.dbContext.UsersGyms
                .Where(ug => ug.IsDeleted == false));

            var service = new ArticleService(
                this.mapper,
                this.mockRepository.Object);

            await service.UnsubscribeUserToGymArticlesAsync(userId, gymId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllActiveArticlesCountByGymIdAsyncShouldWorkProperly()
        {
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveArticlesCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllActiveArticlesCountByGymIdAsyncShouldReturnZeroWhenGymIdIsInvalid(string gymId)
        {
            string validGymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(validGymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(validGymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(validGymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveArticlesCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetArticleDetailsByIdAsyncShouldWorkProperly()
        {
            var articleId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var gymId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Gyms.AddAsync(new Gym
            {
                Id = Guid.Parse(gymId),
                Name = "Test",
                PhoneNumber = "Test",
                Description = "Test",
                Email = "Test",
                LogoUri = "test",
                LogoPublicId = "test",
                WebsiteUrl = "test",
                IsDeleted = false
            });

            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.Parse(articleId),
                    GymId = Guid.Parse(gymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.Parse(gymId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.GetArticleDetailsByIdAsync(articleId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(articleId));
            Assert.IsInstanceOf<ArticleDetailsViewModel>(result);
        }

        [Test]
        public async Task GetArticleByIdAsyncShouldWorkProperly()
        {
            var articleId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.Parse(articleId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.GetArticleByIdAsync(articleId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(articleId));
        }


        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("732fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetArticleByIdAsyncShouldReturnNullWhenIdIsInvalid(string articleId)
        {
            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.GetArticleByIdAsync(articleId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetArticleForEditByIdAsyncShouldWorkProperly()
        {
            var articleId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.Parse(articleId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.GetArticleForEditByIdAsync(articleId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(articleId));
            Assert.IsInstanceOf<EditArticleInputModel>(result);
        }

        [Test]
        public async Task CheckIfArticleExistsByIdAsyncShouldReturnTrueWhenIdIsValid()
        {
            var articleId = "732fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.Parse(articleId),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.CheckIfArticleExistsByIdAsync(articleId);

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("632fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfArticleExistsByIdAsyncShouldReturnFalseWhenIdIsInvalid(string articleId)
        {
            await this.dbContext.Articles.AddRangeAsync(new HashSet<Article>
            {
                new Article
                {
                    Id = Guid.Parse("732fe39a-bc5b-4ea4-b0c5-68b2da06768e"),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = false
                },
                new Article
                {
                    Id = Guid.NewGuid(),
                    Title = "Test",
                    Content = "Test",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Article>())
                .Returns(this.dbContext.Articles
                .Where(a => a.IsDeleted == false));

            var service = new ArticleService(this.mapper, this.mockRepository.Object);

            var result = await service.CheckIfArticleExistsByIdAsync(articleId);

            Assert.IsFalse(result);
        }
    }
}
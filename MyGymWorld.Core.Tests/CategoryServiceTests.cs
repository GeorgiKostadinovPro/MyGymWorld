namespace MyGymWorld.Core.Tests
{
    using Moq;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using MyGymWorld.Core.Services;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Core.Tests.Helpers;

    [TestFixture]
    public class CategoryServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private Mock<IRepository> mockRepository;

        [SetUp]
        public async Task Setup()
        {
            this.mockRepository = new Mock<IRepository>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task GetActiveCatgeoriesAsyncShouldWorkProperly()
        {
            await this.dbContext.Categories.AddRangeAsync(new HashSet<Category>
            { 
                new Category
                {
                    Name = "Test1",
                    IsDeleted = false
                },
                new Category
                {
                    Name = "Test2",
                    IsDeleted = false
                },
                new Category
                {
                    Name = "Test3",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Category>())
                .Returns(this.dbContext.Categories
                .Where(c => c.IsDeleted == false)
                .AsQueryable());

            var service = new CategoryService(this.mockRepository.Object);

            var result = await service.GetActiveCategoriesAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result , typeof(Category));
        }

        [Test]
        public async Task GetCategoryByIdShouldWorkProperly()
        {
            var categoryId = Guid.NewGuid();

            await this.dbContext.Categories.AddRangeAsync(new HashSet<Category>
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Test1",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test2",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test3",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Category>())
                .Returns(this.dbContext.Categories
                .Where(c => c.IsDeleted == false)
                .AsQueryable());

            var service = new CategoryService(this.mockRepository.Object);

            var result = await service.GetCategoryByIdAsync(categoryId.ToString());

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Test1"));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetCategoryByIdShouldReturnNullWhenIdIsInvalid(string categoryId)
        {
            await this.dbContext.Categories.AddRangeAsync(new HashSet<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test1",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test2",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test3",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Category>())
                .Returns(this.dbContext.Categories
                .Where(c => c.IsDeleted == false)
                .AsQueryable());

            var service = new CategoryService(this.mockRepository.Object);

            var result = await service.GetCategoryByIdAsync(categoryId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsSelectListItemsShouldWorkProperly()
        {
            await this.dbContext.Categories.AddRangeAsync(new HashSet<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test1",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test2",
                    IsDeleted = false
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Test3",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Category>())
               .Returns(this.dbContext.Categories
               .Where(c => c.IsDeleted == false)
               .AsQueryable());

            var service = new CategoryService(this.mockRepository.Object);

            var result = await service.GetAllAsSelectListItemsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(SelectListItem));
        }
    }
}

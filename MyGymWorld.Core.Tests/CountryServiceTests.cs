namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;

    [TestFixture]
    public class CountryServiceTests
    {
        private MyGymWorldDbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptions<MyGymWorldDbContext> _options = new DbContextOptionsBuilder<MyGymWorldDbContext>()
                       .UseInMemoryDatabase(databaseName: "TestDb")
                       .Options;

            this.dbContext = new MyGymWorldDbContext(_options);
            await this.dbContext.Database.EnsureCreatedAsync();
            await this.dbContext.Database.EnsureDeletedAsync();
        }

        [Test]
        public async Task GetCountryByIdShouldWorkProperly()
        {
            Country country = new Country
            {
                Name = "Bulgaria",
                CreatedOn = DateTime.UtcNow,
            };

            await this.dbContext.Countries.AddAsync(country);
            await this.dbContext.SaveChangesAsync();  
            
            var mockRepository = new Mock<IRepository>();

            mockRepository
                .Setup(x => x.AllNotDeleted<Country>())
                .Returns(this.dbContext.Countries.AsQueryable());
            
            var service = new CountryService(mockRepository.Object);

            string countryId = country.Id.ToString();

            var result = await service.GetCountryByIdAsync(countryId);

            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(country));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public async Task GetCountryByIdShouldReturnNullWhenIdIsInvalid(string countryId)
        {
            Country country = new Country
            {
                Name = "Bulgaria",
                CreatedOn = DateTime.UtcNow,
            };

            await this.dbContext.Countries.AddAsync(country);
            await this.dbContext.SaveChangesAsync();

            var mockRepository = new Mock<IRepository>();

            mockRepository
                .Setup(x => x.AllNotDeleted<Country>())
                .Returns(this.dbContext.Countries.AsQueryable());

            var service = new CountryService(mockRepository.Object);

            var result = await service.GetCountryByIdAsync(countryId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsyncShouldWorkProperly()
        {
            await this.dbContext.AddRangeAsync(new HashSet<Country>
            {
                new Country
                {
                    Name = "Bulgaria",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Country
                {
                    Name = "France",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Country
                {
                    Name = "USA",
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            var mockRepository = new Mock<IRepository>();
            var mockMapper = new Mock<IMapper>();

            mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Country>())
                .Returns(this.dbContext.Countries.Where(x => x.IsDeleted == false).AsQueryable());

            var service = new CountryService(mockRepository.Object);

            var result = await service.GetAllAsSelectListItemsAsync();

            Assert.That(2, Is.EqualTo(result.Count()));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(SelectListItem));
        }
    }
}

namespace MyGymWorld.Core.Tests
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;

    [TestFixture]
    public class CountryServiceTests
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
        public async Task GetCountryByIdShouldWorkProperly()
        {
            Country country = new Country
            {
                Name = "Bulgaria",
                CreatedOn = DateTime.UtcNow,
            };

            await this.dbContext.Countries.AddAsync(country);
            await this.dbContext.SaveChangesAsync();  
            
            this.mockRepository
                .Setup(x => x.AllNotDeleted<Country>())
                .Returns(this.dbContext.Countries.AsQueryable());
            
            var service = new CountryService(this.mockRepository.Object);

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

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Country>())
                .Returns(this.dbContext.Countries.AsQueryable());

            var service = new CountryService(this.mockRepository.Object);

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

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Country>())
                .Returns(this.dbContext.Countries.Where(x => x.IsDeleted == false).AsQueryable());

            var service = new CountryService(this.mockRepository.Object);

            var result = await service.GetAllAsSelectListItemsAsync();

            Assert.That(2, Is.EqualTo(result.Count()));
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(SelectListItem));
        }
    }
}

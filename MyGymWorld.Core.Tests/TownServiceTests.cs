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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class TownServiceTests
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
        public async Task GetTownByIdShouldWorkProperly()
        {
            Town town1 = new Town
            {
                Id = Guid.NewGuid(),
                Name = "Sofia",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            Town town2 = new Town
            {
                Name = "Plovdiv",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Towns.AddRangeAsync(new List<Town> { town1, town2 });
            await dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Town>())
                .Returns(dbContext.Towns.AsQueryable());

            var service = new TownService(this.mockRepository.Object);

            var result = await service.GetTownByIdAsync(town1.Id.ToString());

            Assert.IsNotNull(result);
            Assert.That(town1.Name, Is.EqualTo(result.Name));
        }


        [Test]
        [TestCase("")]
        [TestCase(null)]
        public async Task GetTownByIdShoulReturnNullForInvalidId(string townId)
        {
            Town town1 = new Town
            {
                Id = Guid.NewGuid(),
                Name = "Sofia",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            Town town2 = new Town
            {
                Name = "Plovdiv",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Towns.AddRangeAsync(new List<Town> { town1, town2 });
            await dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Town>())
                .Returns(dbContext.Towns.AsQueryable());

            var service = new TownService(this.mockRepository.Object);

            var result = await service.GetTownByIdAsync(townId);

            Assert.IsNull(result);
        }


        [Test]
        public async Task CheckIfTownIsPresentByCountryIdAsyncShouldWorkProperly()
        {
            var countryId = Guid.NewGuid();

            Town town1 = new Town
            {
                Id = Guid.NewGuid(),
                Name = "Sofia",
                Population = 123456,
                ZipCode = "123456",
                CountryId = countryId,
                CreatedOn = DateTime.UtcNow
            };

            Town town2 = new Town
            {
                Name = "Plovdiv",
                Population = 123456,
                ZipCode = "123456",
                CountryId = countryId,
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Towns.AddRangeAsync(new List<Town> { town1, town2 });
            await dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Town>())
                .Returns(dbContext.Towns.AsQueryable());

            var service = new TownService(this.mockRepository.Object);

            var result = await service.CheckIfTownIsPresentByCountryIdAsync(town1.Id.ToString(), countryId.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("a130a2c6-5589-48f2-b9d0-36bfeea6ebc5", null)]
        [TestCase(null, "a130a2c6-5589-48f2-b9d0-36bfeea6ebc5")]

        public async Task CheckIfTownIsPresentByCountryIdAsyncShouldReturnFalseWhenDataIsNotValid(string townId, string countryId)
        {
            Town town1 = new Town
            {
                Id = Guid.Parse("a130a2c6-5589-48f2-b9d0-36bfeea6ebc5"),
                Name = "Sofia",
                Population = 123456,
                ZipCode = "123456",
                CountryId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow
            };

            Town town2 = new Town
            {
                Name = "Plovdiv",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Towns.AddRangeAsync(new List<Town> { town1, town2 });
            await dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Town>())
                .Returns(dbContext.Towns.AsQueryable());

            var service = new TownService(this.mockRepository.Object);

            var result = await service.CheckIfTownIsPresentByCountryIdAsync(townId, countryId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetallAsSelectListItemsAsyncShouldWorkProperly()
        {
            var mockRepository = new Mock<IRepository>();

            Town town1 = new Town
            {
                Name = "Sofia",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            Town town2 = new Town
            {
                Name = "Plovdiv",
                Population = 123456,
                ZipCode = "123456",
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Towns.AddRangeAsync(new List<Town> { town1, town2 });
            await dbContext.SaveChangesAsync();

            mockRepository
               .Setup(x => x.AllNotDeletedReadonly<Town>())
               .Returns(this.dbContext.Towns.AsQueryable());

            var service = new TownService(mockRepository.Object);

            var result = await service.GetAllAsSelectListItemsAsync();

            Assert.That(2, Is.EqualTo(result.Count()));
            Assert.That(town2.Name, Is.EqualTo(result.First().Text));
        }
    }
}
